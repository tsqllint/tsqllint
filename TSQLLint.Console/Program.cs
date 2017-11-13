using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Lib.Reporters;

namespace TSQLLint.Console.Standard
{
    public class Program
    {
        [ExcludeFromCodeCoverage]
        public static void Main(string[] args)
        {
            //var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //Trace.Listeners.Add(new Log4netTraceListener(logger, new BooleanSwitch("Logging", "Entire application").Enabled));
            
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
