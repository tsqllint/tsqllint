using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Lib.Reporters;

namespace TSQLLint.Console
{
    public class Program
    {
        [ExcludeFromCodeCoverage]
        public static void Main(string[] args)
        {
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
