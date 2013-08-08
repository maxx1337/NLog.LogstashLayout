using NLog.Targets;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLog.LogstashLayout.Tests
{
    [TestFixture]
    public class NLogTests
    {
        [Test]
        public void TestLogging()
        {
            LogManager.GetCurrentClassLogger().Error("This is a test message");
        }
    }
}
