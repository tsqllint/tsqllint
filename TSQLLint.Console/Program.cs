using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
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
            var debug = Configuration.GetValue<bool>("debug", false);

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

                if(debug){
                    System.Console.WriteLine(exception);
                }
            }
        }
    }
}
