using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Lib.Reporters;
using TSQLLint.Lib.Utility;

namespace TSQLLint.Console
{
    internal class Program
    {
        [ExcludeFromCodeCoverage]
        private static void Main(string[] args)
        {
            var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            Trace.Listeners.Add(new Log4netTraceListener(logger, new BooleanSwitch("Logging", "Entire application").Enabled));
            
            var application = new Application(args, new ConsoleReporter());
            
            try
            {
                application.Run();
            }
            catch (Exception exception)
            {
                System.Console.WriteLine("TSQLLint encountered a problem.");
                Trace.WriteLine(exception);
            }
        }
    }
}
