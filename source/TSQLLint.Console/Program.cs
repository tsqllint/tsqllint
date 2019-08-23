using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.CommandLineOptions;
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

            ICommandLineOptions options = new CommandLineOptions(args);

            IAwaitableReporter reporter = new ConsoleReporter();

            var application = new Application(options, reporter);

            try
            {
                application.Run();

                // wait for the reporter to finish
                reporter.ReportingTask.Wait();
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
