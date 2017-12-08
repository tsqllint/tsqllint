using NSubstitute;
using NUnit.Framework;
using System.Diagnostics;
using TSQLLint.Lib.Utility;
using log4net;

namespace TSQLLint.Tests.UnitTests.Utility
{
   [TestFixture]
   public class LoggingTests
   {
       [Test]
       public void TraceListener_LoggingEnabled_ShouldLog()
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
       public void TraceListener_LoggingDisabled_ShouldNotLog()
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
