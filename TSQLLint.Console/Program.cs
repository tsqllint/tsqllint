using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
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
                #if NET452
                    var thread = new Thread(x => { application.Run(); }, 2500000);
                    thread.Start();
                    thread.Join();
                #elif NETCOREAPP2_0
                    application.Run();
                #endif
            }
            catch (Exception exception)
            {
                System.Console.WriteLine("TSQLLint encountered a problem.");
                Trace.WriteLine(exception);
            }
        }
    }
}
