using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using TSQLLint.Infrastructure.Reporters;

namespace TSQLLint.Console
{
    public class Program
    {
        [ExcludeFromCodeCoverage]
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            var debug = configuration.GetValue("debug", false);
            var application = new Application(args, new ConsoleReporter());

            try
            {
                application.Run();
            }
            catch (Exception exception)
            {
                System.Console.WriteLine("TSQLLint encountered a problem.");

                if (debug)
                {
                    System.Console.WriteLine(exception);
                }
            }
        }
    }
}
