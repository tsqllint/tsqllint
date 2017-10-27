using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Config;

namespace TSQLLint.Tests.UnitTests.Config
{
    public class ConfigReaderTests
    {
        [Test]
        public void ConfigReaderInMemoryConfig()
        {
            // arrange
            var fileSystem = new MockFileSystem();
            var reporter = Substitute.For<IReporter>();

            const string defaultConfigFile = @"
            {
                'rules': {
                    'select-star': 'error',
                    'statement-semicolon-termination': 'warning'
                }
            }";

            // act
            var configReader = new ConfigReader(reporter, fileSystem);
            configReader.LoadConfig(null, defaultConfigFile);

            // assert
            Assert.AreEqual(RuleViolationSeverity.Error, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("statement-semicolon-termination"));
        }

        [Test]
        public void ConfigReaderEmptyConfigFile()
        {
            // arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            var reporter = Substitute.For<IReporter>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem);
            configReader.LoadConfigFromFile(string.Empty);

            // assert
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("statement-semicolon-termination"));
        }

        [Test]
        public void ConfigReaderConfigFileDoesntExist()
        {
            // arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());

            var reporter = Substitute.For<IReporter>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem);
            configReader.LoadConfigFromFile(@"c:\users\someone\.tsqllintrc");

            // assert
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("statement-semicolon-termination"));
        }

        [Test]
        public void ConfigReaderGetRuleSeverity()
        {
            // arrange
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
                }
            });

            var reporter = Substitute.For<IReporter>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem);
            configReader.LoadConfigFromFile(configFilePath);

            // assert
            Assert.AreEqual(RuleViolationSeverity.Error, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("statement-semicolon-termination"));
        }

        [Test]
        public void ConfigReaderNoRulesNoThrow()
        {
            // arrange
            const string configFilePath = @"c:\users\someone\.tsqllintrc-missing-rules";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"{}")
                }
            });

            var reporter = Substitute.For<IReporter>();

            // assert
            Assert.DoesNotThrow(() =>
            {
                // act
                var configReader = new ConfigReader(reporter, fileSystem);
                Assert.IsNotNull(configReader);
            });
        }

        [Test]
        public void ConfigReaderReadBadRuleName()
        {
            // arrange
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
                }
            });

            var reporter = Substitute.For<IReporter>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem);

            // assert
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("foo"), "Rules that dont have a validator should be set to off");
        }

        [Test]
        public void ConfigReaderConfigReadBadRuleSeverity()
        {
            // arrange
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
                }
            });
            var reporter = Substitute.For<IReporter>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem);
            configReader.LoadConfigFromFile(configFilePath);

            // assert
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("select-star"), "Rules that dont have a valid severity should be set to off");
        }

        [Test]
        public void ConfigReaderConfigReadInvalidJson()
        {
            // arrange
            const string configFilePath = @"c:\users\someone\.tsqllintrc-bad-json";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"{")
                }
            });

            var reporter = Substitute.For<IReporter>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem);
            configReader.LoadConfigFromFile(configFilePath);

            // assert
            reporter.Received().Report("Config file is not valid Json.");
        }

        [Test]
        public void ConfigReaderSetupPlugins()
        {
            // arrange
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
                { pluginPath, new MockFileData("") }
            });

            var reporter = Substitute.For<IReporter>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem);
            configReader.LoadConfigFromFile(configFilePath);
            var plugins = configReader.GetPlugins();

            // assert
            Assert.AreEqual(RuleViolationSeverity.Error, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("statement-semicolon-termination"));

            Assert.AreEqual(true, plugins.ContainsKey("my-first-plugin"));
            Assert.AreEqual(true, plugins.ContainsKey("my-second-plugin"));

            Assert.AreEqual("c:/users/someone/my-plugins/my-first-plugin.dll", plugins["my-first-plugin"]);
            Assert.AreEqual("c:/users/someone/my-plugins/my-second-plugin.dll", plugins["my-second-plugin"]);
        }
    }
}
