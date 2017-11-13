using System.Diagnostics;
using log4net;

namespace TSQLLint.Lib.Utility
{
    public class Log4netTraceListener : TraceListener
    {
        private readonly ILog _log;
        private readonly bool LoggingEnabled;

        public Log4netTraceListener(ILog logger, bool loggingEnabled)
        {
            _log = logger;
            LoggingEnabled = loggingEnabled;
        }

        public override void Write(string message)
        {
            if (LoggingEnabled)
            {
                _log.Debug(message);
            }
        }

        public override void WriteLine(string message)
        {
            if (LoggingEnabled)
            {
                _log.Debug(message);
            }
        }
    }
}
