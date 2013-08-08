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
    }
}
