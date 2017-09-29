using System.Diagnostics.CodeAnalysis;
using TSQLLINT_CONSOLE.Reporters;

namespace TSQLLINT_CONSOLE
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