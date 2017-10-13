using System;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Console.Reporters;

namespace TSQLLint.Console
{
    internal class Program
    {
        [ExcludeFromCodeCoverage]
        private static void Main(string[] args)
        {
            var application = new Application(args, new ConsoleReporter());

            try
            {
                application.Run();
            }
            catch (Exception)
            {
                System.Console.WriteLine("TSQLLint encountered a problem.");
            }
        }
    }
}
