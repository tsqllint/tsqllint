using TSQLLINT_CONSOLE.Reporters;

namespace TSQLLINT_CONSOLE
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var application = new Application(args, new ConsoleReporter());
            application.Run();
        }
    }
}