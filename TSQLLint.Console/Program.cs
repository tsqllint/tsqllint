using System;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Lib.Reporters;
using Microsoft.Extensions.Configuration;

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

            var Configuration = builder.Build();
            var debug = Configuration.GetValue("debug", false);
            var application = new Application(args, new ConsoleReporter());

            try
            {
                application.Run();
            }
            catch (Exception exception)
            {
                System.Console.WriteLine("TSQLLint encountered a problem.");

                if(debug){
                    System.Console.WriteLine(exception);
                }
            }
        }
    }
}
