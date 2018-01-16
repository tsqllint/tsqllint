using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using TSQLLint.Tests.Helpers;

namespace TSQLLint.Tests.FunctionalTests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class ConsoleAppTests
    {
        private string testDirectoryPath;

        [SetUp]
        public void Setup()
        {
            testDirectoryPath = TestContext.CurrentContext.WorkDirectory;
        }

        [TestCase(@"", 0)]
        [TestCase(@"-i", 0)]
        [TestCase(@"-l", 0)]
        [TestCase(@"-i -f", 0)]
        [TestCase(@"-p", 0)]
        [TestCase(@"-v", 0)]
        [TestCase(@"-h", 0)]
        [TestCase(@"-c .tsqllintrc", 0)]
        [TestCase(@"invalid", 0)]
        [TestCase(@"c:/foo_invalid.sql", 0)]
        [TestCase(@"-foo", 1)]
        public void NoLintingExitCodeTest(string arguments, int expectedExitCode)
        {
            void OutputHandler(object sender, DataReceivedEventArgs args) { }

            void ErrorHandler(object sender, DataReceivedEventArgs args) { }

            void ExitHandler(object sender, EventArgs args)
            {
                var processExitCode = ((Process)sender).ExitCode;
                Assert.AreEqual(expectedExitCode, processExitCode, $"Exit code should be {expectedExitCode}");
            }

            var process = ConsoleAppTestHelper.GetProcess(arguments, OutputHandler, ErrorHandler, ExitHandler);
            ConsoleAppTestHelper.RunApplication(process);
        }

        [TestCase(@"TestFiles/no-errors.sql", 0)]
        [TestCase(@"TestFiles/with-warnings.sql", 0)]
        [TestCase(@"TestFiles/with-errors.sql", 1)]
        [TestCase(@"TestFiles/invalid-syntax.sql", 0)]
        public void LintingExitCodeTest(string testFile, int expectedExitCode)
        {
            var fileLinted = false;

            void OutputHandler(object sender, DataReceivedEventArgs args)
            {
                if (args.Data != null && args.Data.Contains("Linted 1 files"))
                {
                    fileLinted = true;
                }
            }

            void ErrorHandler(object sender, DataReceivedEventArgs args) { }

            void ExitHandler(object sender, EventArgs args)
            {
                var processExitCode = ((Process)sender).ExitCode;
                Assert.AreEqual(expectedExitCode, processExitCode, $"Exit code should be {expectedExitCode}");
            }

            var path = Path.GetFullPath(Path.Combine(testDirectoryPath, $@"FunctionalTests/{testFile}"));
            var configFilePath = Path.GetFullPath(Path.Combine(testDirectoryPath, @"FunctionalTests/.tsqllintrc"));

            var process = ConsoleAppTestHelper.GetProcess($"-c {configFilePath} {path}", OutputHandler, ErrorHandler, ExitHandler);
            ConsoleAppTestHelper.RunApplication(process);

            Assert.IsTrue(fileLinted);
        }

        [TestCase(@"-l", "Loaded plugin 'TSQLLint.Tests.UnitTests.PluginHandler.TestPlugin'", 0)]
        public void LoadPluginTest(string testArgs, string expectedMessage, int expectedExitCode)
        {
            var pluginLoaded = false;

            void OutputHandler(object sender, DataReceivedEventArgs args)
            {
                if (args.Data != null && args.Data.Contains(expectedMessage))
                {
                    pluginLoaded = true;
                }
            }

            void ErrorHandler(object sender, DataReceivedEventArgs args) { }

            void ExitHandler(object sender, EventArgs args)
            {
                var processExitCode = ((Process)sender).ExitCode;
                Assert.AreEqual(expectedExitCode, processExitCode, $"Exit code should be {expectedExitCode}");
            }

            var configFilePath = Path.GetFullPath(Path.Combine(testDirectoryPath, @"FunctionalTests/.tsqllintrc-plugins"));
            var jsonString = File.ReadAllText(configFilePath);
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);
            jsonObject["plugins"]["test-plugin"] = ConsoleAppTestHelper.TestPluginPath;
            var updatedConfigFilePath = Path.Combine(testDirectoryPath, @"FunctionalTests/.tsqllintrc-plugins-updated");
            File.WriteAllText(updatedConfigFilePath, jsonObject.ToString());

            var process = ConsoleAppTestHelper.GetProcess($"-c {updatedConfigFilePath} {testArgs}", OutputHandler, ErrorHandler, ExitHandler);
            ConsoleAppTestHelper.RunApplication(process);

            Assert.IsTrue(pluginLoaded);

            // remove updated plugin config file
            File.Delete(updatedConfigFilePath);
        }
    }
}
