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
            DataReceivedEventHandler outputHandler = (sender, args) => { };
            DataReceivedEventHandler errorHandler = (sender, args) => { };

            EventHandler exitHandler = (sender, args) =>
            {
                var processExitCode = ((Process)sender).ExitCode;
                Assert.AreEqual(expectedExitCode, processExitCode, string.Format("Exit code should be {0}", expectedExitCode));
            };

            var process = ConsoleAppTestHelper.GetProcess(arguments, outputHandler, errorHandler, exitHandler);
            ConsoleAppTestHelper.RunApplication(process);
        }

        [TestCase(@"\TestFiles\integration-test-one.sql", 1)]
        public void FileLintingTest(string testFile, int expectedExitCode)
        {
            var fileLinted = false;

            DataReceivedEventHandler outputHandler = (sender, args) =>
            {
                if (args.Data != null && args.Data.Contains("Linted 1 files in"))
                {
                    fileLinted = true;
                }
            };

            DataReceivedEventHandler errorHandler = (sender, args) => { };

            EventHandler exitHandler = (sender, args) =>
            {
                var processExitCode = ((Process)sender).ExitCode;
                Assert.AreEqual(expectedExitCode, processExitCode, string.Format("Exit code should be {0}", expectedExitCode));
            };

            var path = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, string.Format(@"..\..\IntegrationTests\{0}", testFile)));

            var process = ConsoleAppTestHelper.GetProcess(path, outputHandler, errorHandler, exitHandler);
            ConsoleAppTestHelper.RunApplication(process);

            Assert.IsTrue(fileLinted);
        }
    }
}
