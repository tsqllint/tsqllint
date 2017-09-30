using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace TSQLLINT_LIB_TESTS.FunctionalTests
{
    internal delegate void ExitProcess_DEL(object sender, EventArgs args);

    [TestFixture]
    public class ConsoleAppTests
    {
        private string _ApplicationPath;

        private string ApplicationPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_ApplicationPath))
                {
                    var workingDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory);
                    _ApplicationPath = string.Format("{0}\\TSQLLINT_CONSOLE.exe", workingDirectory);
                }

                return _ApplicationPath;
            }
        }

        public Process GetProcess(string arguments, DataReceivedEventHandler OutputHandler, DataReceivedEventHandler ErrorHandler, EventHandler ExitHandler)
        {
            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = ApplicationPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += OutputHandler;
            process.ErrorDataReceived += ErrorHandler;
            process.Exited += ExitHandler;
            return process;
        }

        public void RunApplication(Process process)
        {
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }

        [TestCase(@"-i", 0)]
        [TestCase(@"-p", 0)]
        [TestCase(@"-v", 0)]
        [TestCase(@"-h", 0)]
        [TestCase(@"-i -f", 0)]
        [TestCase(@"-c .tsqllintrc", 0)]
        [TestCase(@"-c .tsqllintrc", 0)]
        [TestCase(@"invalid.sql", 0)]
        [TestCase(@"c:\foo_invalid.sql", 0)]
        [TestCase(@"-foo", 1)]
        public void ExitCodeTest(string arguments, int expectedExitCode)
        {
            DataReceivedEventHandler outputHandler = (sender, args) =>
            {
                Console.WriteLine(args.Data);
            };

            DataReceivedEventHandler errorHandler = (sender, args) =>
            {
                Console.WriteLine(args.Data);
            };

            EventHandler exitHandler = (sender, args) =>
            {
                var processExitCode = ((Process)sender).ExitCode;
                Assert.AreEqual(expectedExitCode, processExitCode, "Exit code should be zero when no errors occur");
            };

            var process = GetProcess(arguments, outputHandler, errorHandler, exitHandler);
            RunApplication(process);
        }
    }
}