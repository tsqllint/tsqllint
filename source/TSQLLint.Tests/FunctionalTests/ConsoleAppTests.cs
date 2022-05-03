using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
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

        [TestCase(@"-i", 0)]
        [TestCase(@"-l", 0)]
        [TestCase(@"-i -f", 0)]
        [TestCase(@"-p", 0)]
        [TestCase(@"-v", 0)]
        [TestCase(@"-h", 0)]
        [TestCase(@"-c .tsqllintrc", 0)]
        [TestCase(@"invalid", 1)]
        [TestCase(@"c:/foo_invalid.sql", 1)]
        [TestCase(@"-foo", 1)]
        [TestCase(@"", 1)]
        public void NoLintingPerformedExitCodeTest(string arguments, int expectedExitCode)
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
        [TestCase(@"TestFiles/invalid-syntax.sql", 1)]
        [TestCase(@"TestFiles/linting-disabled.sql", 0)]
        public void LintingPerformedExitCodeTest(string testFile, int expectedExitCode)
        {
            void OutputHandler(object sender, DataReceivedEventArgs args)
            {
                TestContext.Out.WriteLine(args.Data);
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
        }

        [TestCase(@"TestFiles/with-fixable-errors.sql", false)]
        [TestCase(@"TestFiles/with-fixable-errors.sql", true)]
        public void LintingErrorsFixedExitCodeTest(string testFile, bool withFix)
        {
            void OutputHandler(object sender, DataReceivedEventArgs args)
            {
                TestContext.Out.WriteLine(args.Data);
            }

            void ErrorHandler(object sender, DataReceivedEventArgs args) { }

            void FixExitHandler(object sender, EventArgs args)
            {
                var processExitCode = ((Process)sender).ExitCode;
                Assert.AreEqual(1, processExitCode, $"Exit code should be {1}");
            }

            void ValidateFixExitHandler(object sender, EventArgs args)
            {
                var expectedExistCode = withFix ? 0 : 1;
                var processExitCode = ((Process)sender).ExitCode;
                Assert.AreEqual(expectedExistCode, processExitCode, $"Exit code should be {expectedExistCode}");
            }

            var testPath = Path.GetFullPath(Path.Combine(testDirectoryPath, $@"FunctionalTests/{testFile}"));
            var testOutputPath = testPath.Replace(".sql", $".fixed-{withFix}.sql");

            var testFileContent = File.ReadAllText(testPath);
            File.WriteAllText(testOutputPath, testFileContent);

            var configFilePath = Path.GetFullPath(Path.Combine(testDirectoryPath, @"FunctionalTests/.tsqllintrc"));

            var fixProcess = ConsoleAppTestHelper.GetProcess($"{(withFix ? "-x" : string.Empty)} -c {configFilePath} {testOutputPath}", OutputHandler, ErrorHandler, FixExitHandler);

            var validateFixProcess = ConsoleAppTestHelper.GetProcess($" -c {configFilePath} {testOutputPath}", OutputHandler, ErrorHandler, ValidateFixExitHandler);

            ConsoleAppTestHelper.RunApplication(fixProcess);
            ConsoleAppTestHelper.RunApplication(validateFixProcess);
        }

        [TestCase(@"TestFiles/with-tabs.sql", "prefer-tabs : Should use spaces rather than tabs", 0)]
        [TestCase(@"TestFiles/with-spaces.sql", "Loaded plugin: 'TSQLLint.Tests.UnitTests.PluginHandler.TestPlugin'", 0)]
        public void LoadPluginTest(string testFile, string expectedMessage, int expectedExitCode)
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

            var testPath = Path.GetFullPath(Path.Combine(testDirectoryPath, $@"FunctionalTests/{testFile}"));

            var configFilePath = Path.GetFullPath(Path.Combine(testDirectoryPath, @"FunctionalTests/.tsqllintrc-plugins"));
            var jsonString = File.ReadAllText(configFilePath);
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);
            jsonObject["plugins"]["test-plugin"] = ConsoleAppTestHelper.TestPluginPath;
            var updatedConfigFilePath = Path.Combine(testDirectoryPath, @"FunctionalTests/.tsqllintrc-plugins-updated");
            File.WriteAllText(updatedConfigFilePath, jsonObject.ToString());

            var process = ConsoleAppTestHelper.GetProcess($"-c {updatedConfigFilePath} -l {testPath}", OutputHandler, ErrorHandler, ExitHandler);
            ConsoleAppTestHelper.RunApplication(process);

            Assert.IsTrue(pluginLoaded);

            // remove updated plugin config file
            File.Delete(updatedConfigFilePath);
        }
    }
}
