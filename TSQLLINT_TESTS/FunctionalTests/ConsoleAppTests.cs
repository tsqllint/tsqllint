using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace TSQLLINT_LIB_TESTS.FunctionalTests
{
    public delegate void ExitProcessDel(object sender, EventArgs args);

    [TestFixture]
    public class ConsoleAppTests
    {
        private string _applicationPath;

        private string ApplicationPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_applicationPath))
                {
                    var workingDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory);
                    _applicationPath = string.Format("{0}\\TSQLLINT_CONSOLE.exe", workingDirectory);
                }
                return _applicationPath;
            }
        }

        public Process GetProcess(string arguments, DataReceivedEventHandler outputHandler, DataReceivedEventHandler errorHandler, EventHandler exitHandler)
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

            process.OutputDataReceived += outputHandler;
            process.ErrorDataReceived += errorHandler;
            process.Exited += exitHandler;
            return process;
        }

        public void RunApplication(Process process)
        {
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }

        [TestCase("-i", 0)]
        [TestCase("-p", 0)]
        [TestCase("-v", 0)]
        [TestCase("-h", 0)]
        [TestCase("-i -f", 0)]
        [TestCase("-c .tsqllintrc", 0)]
        [TestCase("-c .tsqllintrc", 0)]
        [TestCase("foo.sql", 0)]
        [TestCase("-foo", 1)]
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