using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using TSQLLint.Tests.Helpers;

namespace TSQLLint.Tests.FunctionalTests
{
    [TestFixture]
    public class ConsoleAppTests
    {
        [TestCase(@"-i", 0)]
        [TestCase(@"-i -f", 0)]
        [TestCase(@"-p", 0)]
        [TestCase(@"-v", 0)]
        [TestCase(@"-h", 0)]
        [TestCase(@"-c .tsqllintrc", 0)]
        [TestCase(@"invalid", 0)]
        [TestCase(@"c:\foo_invalid.sql", 0)]
        [TestCase(@"-foo", 1)]
        public void NoLintingExitCodeTest(string arguments, int expectedExitCode)
        {
            void OutputHandler(object sender, DataReceivedEventArgs args)
            {
            }

            void ErrorHandler(object sender, DataReceivedEventArgs args)
            {
            }

            void ExitHandler(object sender, EventArgs args)
            {
                var processExitCode = ((Process)sender).ExitCode;
                Assert.AreEqual(expectedExitCode, processExitCode, $"Exit code should be {expectedExitCode}");
            }

            var process = ConsoleAppTestHelper.GetProcess(arguments, OutputHandler, ErrorHandler, ExitHandler);
            ConsoleAppTestHelper.RunApplication(process);
        }

        [TestCase(@"\TestFiles\no-errors.sql", 0)]
        [TestCase(@"\TestFiles\with-warnings.sql", 0)]
        [TestCase(@"\TestFiles\with-errors.sql", 1)]
        public void LintingExitCodeTest(string testFile, int expectedExitCode)
        {
            var fileLinted = false;

            void OutputHandler(object sender, DataReceivedEventArgs args)
            {
                if (args.Data != null && args.Data.Contains("Linted 1 files in"))
                {
                    fileLinted = true;
                }
            }

            void ErrorHandler(object sender, DataReceivedEventArgs args)
            {
            }

            void ExitHandler(object sender, EventArgs args)
            {
                var processExitCode = ((Process)sender).ExitCode;
                Assert.AreEqual(expectedExitCode, processExitCode, $"Exit code should be {expectedExitCode}");
            }

            var path = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, $@"..\..\FunctionalTests\{testFile}"));
            var configFilePath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\FunctionalTests\.tsqllintrc"));
            
            var process = ConsoleAppTestHelper.GetProcess($"-c {configFilePath} {path}", OutputHandler, ErrorHandler, ExitHandler);
            ConsoleAppTestHelper.RunApplication(process);

            Assert.IsTrue(fileLinted);
        }
    }
}
