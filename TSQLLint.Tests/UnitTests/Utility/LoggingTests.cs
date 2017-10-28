using System.Diagnostics;
using log4net;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Lib.Utility;

namespace TSQLLint.Tests.UnitTests.Utility
{
    [TestFixture]
    public class LoggingTests
    {
        [Test]
        public void ShouldLogWhenLoggingEnabled()
        {
            // arrange
            var mockLogger = Substitute.For<ILog>();
            var loggingEnabled = true;
            var traceListener = new Log4netTraceListener(mockLogger, loggingEnabled);

            // act
            traceListener.Write("Write");
            traceListener.WriteLine("Writeline");

            // assert
            mockLogger.Received().Debug("Write");
            mockLogger.Received().Debug("Writeline");
        }

        [Test]
        public void ShouldNotLogWhenLoggingDisabled()
        {
            // arrange
            var mockLogger = Substitute.For<ILog>();
            var loggingEnabled = false;
            var traceListener = new Log4netTraceListener(mockLogger, loggingEnabled);

            // act
            traceListener.Write("Write");
            traceListener.WriteLine("Writeline");

            // assert
            mockLogger.DidNotReceive().Debug("Write");
            mockLogger.DidNotReceive().Debug("Writeline");
        }
    }
}
