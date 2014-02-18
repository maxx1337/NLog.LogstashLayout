using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLog.LogstashLayout.Tests
{
    [TestFixture]
    public class JsonEventLayoutRendererTests
    {
        [Test]
        public void Append_withNiceLogEvent_returnsNiceJson()
        {
            // arrange
            JsonEventLayoutRenderer layout = new JsonEventLayoutRenderer();
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Warn, "UnitTestLogger", @"My message
with

some newlines and german öäüß Sonderzeichen!");

            // act
            string result = layout.Render(logEvent);

            // assert something here
            Console.WriteLine(result);
        }


        [Test]
        public void Append_withNiceLogEventAndException_returnsNiceJson()
        {
            // arrange
            JsonEventLayoutRenderer layout = new JsonEventLayoutRenderer();
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Warn, "UnitTestLogger", @"My message
with

some newlines and german öäüß Sonderzeichen!");


            logEvent.Exception = new InvalidOperationException("Something's wrong here");

            // act
            string result = layout.Render(logEvent);

            // assert something here
            Console.WriteLine(result);
        }

        [Test]
        public void Append_withNiceLogEventAndLongMessageAndEmptySuffix_returnsFullMessage()
        {
            // arrange
            JsonEventLayoutRenderer layout = new JsonEventLayoutRenderer();
            layout.EnableShortMessage = false; // is default
            layout.ShortMessageLength = 20;
            layout.AppendToShortenedMessage = String.Empty;
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Warn, "UnitTestLogger", @"My message with some newlines and german öäüß Sonderzeichen!");
            logEvent.Exception = new InvalidOperationException("Something's wrong here");

            // act
            string result = layout.Render(logEvent);

            // full_message is null, should not be in json
            Assert.IsFalse(result.Contains("full_message"));

            JsonEvent theEvent = JsonConvert.DeserializeObject<JsonEvent>(result);
            // full log message
            Assert.AreEqual("My message with some newlines and german öäüß Sonderzeichen!", theEvent.ShortMessage);
            Assert.IsNull(theEvent.FullMessage);
        }

        [Test]
        public void Append_withNiceLogEventAndLongMessageAndEmptySuffix_returnsShortenedMessage()
        {
            // arrange
            JsonEventLayoutRenderer layout = new JsonEventLayoutRenderer();
            layout.EnableShortMessage = true;
            layout.ShortMessageLength = 20;
            layout.AppendToShortenedMessage = String.Empty;
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Warn, "UnitTestLogger", @"My message with some newlines and german öäüß Sonderzeichen!");
            logEvent.Exception = new InvalidOperationException("Something's wrong here");

            // act
            string result = layout.Render(logEvent);

            // full_message is not null, should be in json
            Assert.IsTrue(result.Contains("full_message"));

            // assert something here
            JsonEvent theEvent = JsonConvert.DeserializeObject<JsonEvent>(result);
            Assert.AreEqual("My message with some", theEvent.ShortMessage);
        }

        [Test]
        public void Append_withNiceLogEventAndLongMessageAndNullSuffix_returnsShortenedMessage()
        {
            // arrange
            JsonEventLayoutRenderer layout = new JsonEventLayoutRenderer();
            layout.EnableShortMessage = true;
            layout.ShortMessageLength = 20;
            layout.AppendToShortenedMessage = String.Empty;
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Warn, "UnitTestLogger", @"My message with some newlines and german öäüß Sonderzeichen!");
            logEvent.Exception = new InvalidOperationException("Something's wrong here");

            // act
            string result = layout.Render(logEvent);

            // assert something here
            JsonEvent theEvent = JsonConvert.DeserializeObject<JsonEvent>(result);
            Assert.AreEqual("My message with some", theEvent.ShortMessage);
        }

        [Test]
        public void Append_withNiceLogEventAndLongMessageAndSomeSuffix_returnsShortenedMessageWithSuffix()
        {
            // arrange
            JsonEventLayoutRenderer layout = new JsonEventLayoutRenderer();
            layout.EnableShortMessage = true;
            layout.ShortMessageLength = 20;
            layout.AppendToShortenedMessage = "___";
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Warn, "UnitTestLogger", @"My message with some newlines and german öäüß Sonderzeichen!");
            logEvent.Exception = new InvalidOperationException("Something's wrong here");

            // act
            string result = layout.Render(logEvent);

            // assert something here
            JsonEvent theEvent = JsonConvert.DeserializeObject<JsonEvent>(result);
            Assert.AreEqual("My message with s___", theEvent.ShortMessage);
        }

        [Test]
        public void Append_withProperties_returnsProperties()
        {
            // arrange
            JsonEventLayoutRenderer layout = new JsonEventLayoutRenderer();
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Warn, "UnitTestLogger", @"My message with Properties!");
            logEvent.Properties["CustomProp"] = "myProp";

            // act
            string result = layout.Render(logEvent);

            // assert something here
            JsonEvent theEvent = JsonConvert.DeserializeObject<JsonEvent>(result);
            Assert.AreEqual("My message with Properties!", theEvent.ShortMessage);
            Assert.AreEqual(1, theEvent.Fields.Properties.Count);
            Assert.AreEqual("myProp", theEvent.Fields.Properties["CustomProp"]);
        }
    }
}
