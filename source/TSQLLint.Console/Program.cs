using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TSQLLint.Infrastructure.Reporters;

namespace TSQLLint.Console
{
    public class Program
    {
        [ExcludeFromCodeCoverage]
        public static void Main(string[] args)
        {
            try
            {
                System.Console.WriteLine("Running TSQLLint.");
                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json");

                var configuration = builder.Build();
                var application = new Application(args, new ConsoleReporter());

                application.Run();

                Task.Run(() =>
                {
                    while (NonBlockingConsole.messageQueue.Count > 0) { }
                }).Wait();
            }
            catch (Exception exception)
            {
                System.Console.WriteLine("TSQLLint encountered a problem.");
                System.Console.WriteLine(exception);
            }
        }
    }
}
