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
                //var thread = new Thread(x => { application.Run(); }, 5000000);
                //thread.Start();
                //thread.Join();
            }
            catch (Exception exception)
            {
                System.Console.WriteLine("TSQLLint encountered a problem.");
                Trace.WriteLine(exception);
            }
        }
    }
}
