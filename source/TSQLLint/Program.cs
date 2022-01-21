using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TSQLLint.Infrastructure.Reporters;

namespace TSQLLint
{
    public class Program
    {
        [ExcludeFromCodeCoverage]
        public static void Main(string[] args)
        {
            try
            {
                NonBlockingConsole.WriteLine("running tsqllint");

                var application = new Application(args, new ConsoleReporter());
                application.Run();

                Task.Run(() =>
                {
                    while (NonBlockingConsole.messageQueue.Count > 0) { }
                }).Wait();
            }
            catch (Exception exception)
            {
                Console.WriteLine("TSQLLint encountered a problem.");
                Console.WriteLine(exception);
            }
        }
    }
}
