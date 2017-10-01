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
            application.Run();
        }
    }
}