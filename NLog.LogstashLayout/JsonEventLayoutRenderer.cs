using Newtonsoft.Json;
using NLog.LayoutRenderers;
using NLog.Layouts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace NLog.LogstashLayout
{
    [LayoutRenderer("json_event")]
    public class JsonEventLayoutRenderer : LayoutRenderer
    {
        private string _correlationContextKey = "__correlationContext__";
        private int _shortMessageLength = 200;
        private string _appendToShortenedMessage = "...";

        public string CorrelationContextKey
        {
            get
            {
                return _correlationContextKey;
            }
            set
            {
                _correlationContextKey = value;
            }
        }

        public int ShortMessageLength
        {
            get
            {
                return _shortMessageLength;
            }
            set
            {
                _shortMessageLength = value;
            }
        }

        public string AppendToShortenedMessage
        {
            get
            {
                return _appendToShortenedMessage;
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                _appendToShortenedMessage = value;
            }
        }

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            JsonEvent jEvent = new JsonEvent
            {
                ShortMessage = ShortenMessage(logEvent.FormattedMessage),
                FullMessage = logEvent.FormattedMessage,
                SourceHost = Environment.MachineName,
                Timestamp = logEvent.TimeStamp.ToUniversalTime(),
            };

            AddFieldInfo(logEvent, jEvent);
            AddExceptionInfo(logEvent, jEvent);
            builder.Append(SerializeToJson(jEvent));
        }

        private string ShortenMessage(string fullMessage)
        {
            if (fullMessage == null)
            {
                return null;
            }
            if (fullMessage.Length <= ShortMessageLength)
            {
                return fullMessage;
            }
            return fullMessage.Substring(0, ShortMessageLength - AppendToShortenedMessage.Length) + AppendToShortenedMessage;
        }

        private string SerializeToJson(JsonEvent jEvent)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(jEvent, Formatting.None, settings);
        }

        private void AddFieldInfo(LogEventInfo logEvent, JsonEvent jEvent)
        {
            jEvent.Fields = new FieldInfo
            {
                Level = logEvent.Level.Name,
                LoggerName = logEvent.LoggerName,
                AppDomainName = AppDomain.CurrentDomain.FriendlyName,
                UserName = Thread.CurrentPrincipal.Identity.Name,
                ProcessId = Process.GetCurrentProcess().Id,
                ProcessName = Process.GetCurrentProcess().ProcessName,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                ThreadName = Thread.CurrentThread.Name,
                CorrelationContext = GetCorrelationContext(logEvent),
                Properties = logEvent.Properties,
            };
        }

        private static void AddExceptionInfo(LogEventInfo logEvent, JsonEvent jEvent)
        {
            if (logEvent.Exception != null)
            {
                jEvent.Fields.ExceptionInfo = new ExceptionInfo
                {
                    ExceptionMessage = logEvent.Exception.Message,
                    ExceptionType = logEvent.Exception.GetType().FullName,
                    StackTrace = logEvent.StackTrace == null ? null : logEvent.StackTrace.ToString(),
                    ExceptionDump = logEvent.Exception.ToString()
                };
            }
        }

        private string GetCorrelationContext(LogEventInfo logEvent)
        {
            object cc;
            if (logEvent.Properties.TryGetValue("_gfkCorrelationContext_", out cc))
            {
                return cc.ToString();
            }
            return null;
        }

    }

    internal class JsonEvent
    {
        [JsonProperty("@source_host")]
        public string SourceHost { get; set; }

        [JsonProperty("@message")]
        public string ShortMessage { get; set; }

        [JsonProperty("@full_message")]
        public string FullMessage { get; set; }


        [JsonProperty("@timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("@fields")]
        public FieldInfo Fields { get; set; }
    }

    internal class FieldInfo
    {
        [JsonProperty("exception")]
        public ExceptionInfo ExceptionInfo { get; set; }
        public string LoggerName { get; set; }
        public string CorrelationContext { get; set; }
        public string Level { get; set; }
        public string ThreadName { get; set; }
        public string ProcessName { get; set; }
        public int ThreadId { get; set; }
        public int ProcessId { get; set; }
        public string UserName { get; set; }
        public string AppDomainName { get; set; }
        public string NestedDiagnosticsContext { get; set; }
        public string MappedDiagnosticsContext { get; set; }
        public IDictionary<object, object> Properties { get; set; }
    }

    internal class ExceptionInfo
    {
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionDump { get; set; }
        public string StackTrace { get; set; }
    }
}
