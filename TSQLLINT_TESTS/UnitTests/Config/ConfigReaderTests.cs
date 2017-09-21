using System.Collections.Generic;
using NUnit.Framework;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using TSQLLINT_COMMON;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.UnitTests.Config
{
    public class ConfigReaderTests
    {
        [Test]
        public void ConfigReader_GetRuleSeverity()
        {
            //arrange
            const string configFilePath = @"c:\users\someone\.tsqllintrc";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"
                    {
                        'rules': {
                            'select-star': 'error',
                            'statement-semicolon-termination': 'warning'
                        }
                    }")
                },
            });

            var reporter = Substitute.For<IReporter>();

            //act
            var configReader = new ConfigReader(reporter, fileSystem, configFilePath);

            //assert
            Assert.IsTrue(configReader.ConfigIsValid);
            Assert.AreEqual(RuleViolationSeverity.Error, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("statement-semicolon-termination"));
        }

        [Test]
        public void ConfigReader_NoRulesNoThrow()
        {
            //arrange
            const string configFilePath = @"c:\users\someone\.tsqllintrc-missing-rules";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"{}")
                },
            });

            var reporter = Substitute.For<IReporter>();

            //assert
            Assert.DoesNotThrow(() =>
            {
                //act
                var configReader = new ConfigReader(reporter, fileSystem, configFilePath);
                Assert.IsTrue(configReader.ConfigIsValid);
            });
        }

        [Test]
        public void ConfigReader_ReadBadRuleName()
        {
            //arrange
            const string configFilePath = @"c:\users\someone\.tsqllintrc";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"
                    {
                        'rules': {
                            'select-star': 'error',
                            'statement-semicolon-termination': 'warning'
                        }
                    }")
                },
            });

            var reporter = Substitute.For<IReporter>();

            //act
            var configReader = new ConfigReader(reporter, fileSystem, configFilePath);

            //assert
            Assert.IsTrue(configReader.ConfigIsValid);
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("foo"), "Rules that dont have a validator should be set to off");
        }

        [Test]
        public void ConfigReader_ConfigReadBadRuleSeverity()
        {
            //arrange
            const string configFilePath = @"c:\users\someone\.tsqllintrc-bad-severity";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"
                    {
                        'rules': {
                            'select-star': 'foo'
                        }
                    }")
                },
            });
            var reporter = Substitute.For<IReporter>();

            //act
            var configReader = new ConfigReader(reporter, fileSystem, configFilePath);

            //assert
            Assert.IsTrue(configReader.ConfigIsValid);
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("select-star"), "Rules that dont have a valid severity should be set to off");
        }

        [Test]
        public void ConfigReader_ConfigReadEmptyFile()
        {
            var reporter = Substitute.For<IReporter>();
            var ConfigReader = new ConfigReader(reporter, "");
            Assert.IsFalse(ConfigReader.ConfigIsValid, "Empty config files should not be flagged as valid");
        }

        [Test]
        public void ConfigReader_ConfigReadInvalidJson()
        {
            //arrange
            const string configFilePath = @"c:\users\someone\.tsqllintrc-bad-json";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"{")
                },
            });

            var reporter = Substitute.For<IReporter>();

            //act
            var configReader = new ConfigReader(reporter, fileSystem, configFilePath);

            //assert
            Assert.IsFalse(configReader.ConfigIsValid, "Invalid Json should be flagged as invalid");
            reporter.Received().Report("Config file is not valid Json.");
        }

        [Test]
        public void ConfigReader_SetupPlugins()
        {
            //arrange
            const string configFilePath = @"c:\users\someone\.tsqllintrc";
            const string pluginPath = @"c:\users\someone\my-plugins\foo.dll";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"
                    {
                        'rules': {
                            'select-star': 'error',
                            'statement-semicolon-termination': 'warning'
                        },
                        'plugins': {
                            'my-first-plugin': 'c:/users/someone/my-plugins/my-first-plugin.dll',
                            'my-second-plugin': 'c:/users/someone/my-plugins/my-second-plugin.dll'
                        }
                    }")
                },
                {pluginPath, new MockFileData("")}
            });

            var reporter = Substitute.For<IReporter>();

            //act
            var configReader = new ConfigReader(reporter, fileSystem, configFilePath);
            var plugins = configReader.GetPlugins();

            //assert
            Assert.IsTrue(configReader.ConfigIsValid);

            Assert.AreEqual(RuleViolationSeverity.Error, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("statement-semicolon-termination"));

            Assert.AreEqual(true, plugins.ContainsKey("my-first-plugin"));
            Assert.AreEqual(true, plugins.ContainsKey("my-second-plugin"));

            Assert.AreEqual("c:/users/someone/my-plugins/my-first-plugin.dll", plugins["my-first-plugin"]);
            Assert.AreEqual("c:/users/someone/my-plugins/my-second-plugin.dll", plugins["my-second-plugin"]);
        }
    }
}