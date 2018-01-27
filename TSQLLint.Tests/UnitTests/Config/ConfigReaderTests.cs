using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Config;
using TSQLLint.Lib.Config.Interfaces;

namespace TSQLLint.Tests.UnitTests.Config
{
    [TestFixture]
    public class ConfigReaderTests
    {
        [SetUp]
        [ExcludeFromCodeCoverage]
        public void Setup()
        {
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                Assert.Ignore("Tests ignored on osx or linux until https://github.com/tathamoddie/System.IO.Abstractions/issues/252 is resolved");
            }
        }

        [Test]
        public void ConfigReaderInMemoryConfig()
        {
            // arrange
            var fileSystem = new MockFileSystem();
            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
            configReader.LoadConfig(string.Empty); // load config from memory
            configReader.ListPlugins();

            // assert
            Assert.AreEqual(RuleViolationSeverity.Error, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Error, configReader.GetRuleSeverity("semicolon-termination"));
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("fake-rule"));
            Assert.IsTrue(configReader.IsConfigLoaded);
            reporter.Received().Report("Did not find any plugins");
        }

        [Test]
        public void ConfigReaderEmptyConfigFile()
        {
            // arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);

            // assert
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("statement-semicolon-termination"));
            Assert.IsFalse(configReader.IsConfigLoaded);
        }

        [Test]
        public void ConfigReaderConfigFileDoesntExist()
        {
            // arrange
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
            configReader.LoadConfig(@"c:\users\someone\.tsqllintrc");

            // assert
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("statement-semicolon-termination"));
            Assert.IsFalse(configReader.IsConfigLoaded);
        }

        [Test]
        public void ConfigReaderLoadsConfigsFromUserProfile()
        {
            // arrange
            var configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".tsqllintrc");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"
                    {
                        'rules': {
                            'select-star': 'warning',
                            'statement-semicolon-termination': 'warning'
                        }
                    }")
                }
            });

            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
            configReader.LoadConfig(null);

            // assert
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("statement-semicolon-termination"));
            Assert.IsTrue(configReader.IsConfigLoaded);
        }

        [Test]
        public void ConfigReaderLoadsConfigsFromLocal()
        {
            // arrange
            var localConfigFile = Path.Combine(TestContext.CurrentContext.TestDirectory, ".tsqllintrc");
            var fileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData>
            {
                {
                    // should ignore config files in user profile when local config exists
                    @"C:\Users\User\.tsqllintrc", new MockFileData(@"
                    {
                        'rules': {
                            'select-star': 'off',
                            'statement-semicolon-termination': 'warning'
                        }
                    }")
                },
                {
                    localConfigFile, new MockFileData(@"
                    {
                        'rules': {
                            'select-star': 'warning',
                            'statement-semicolon-termination': 'warning'
                        }
                    }")
                }
            }, TestContext.CurrentContext.TestDirectory);

            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
            configReader.LoadConfig(null);

            // assert
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("statement-semicolon-termination"));
            Assert.IsTrue(configReader.IsConfigLoaded);
        }

        [Test]
        public void ConfigReaderLoadsConfigsEnvironmentVariable()
        {
            // arrange
            var testConfigFile = @"c:\foo\.tsqllintrc";

            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    {
                        // should ignore config files in user profile when local config exists
                        testConfigFile, new MockFileData(@"
                        {
                            'rules': {
                                'select-star': 'off',
                                'statement-semicolon-termination': 'warning'
                            }
                        }")
                    },
                    {
                        // should ignore config files in user profile when local config exists
                        @"C:\Users\User\.tsqllintrc", new MockFileData(@"
                        {
                            'rules': {
                                'select-star': 'error',
                                'statement-semicolon-termination': 'error'
                            }
                        }")
                    },
                }, TestContext.CurrentContext.TestDirectory);

            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();
            environmentWrapper.GetEnvironmentVariable("tsqllintrc").Returns(testConfigFile);

            // act
            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
            configReader.LoadConfig(null);

            // assert
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("statement-semicolon-termination"));
            Assert.IsTrue(configReader.IsConfigLoaded);
        }

        [Test]
        public void ConfigReaderGetRuleSeverity()
        {
            // arrange
            const string configFilePath = @"C:\Users\User\.tsqllintrc";
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
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);

            // assert
            Assert.AreEqual(RuleViolationSeverity.Error, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("statement-semicolon-termination"));
            Assert.IsTrue(configReader.IsConfigLoaded);
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
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // assert
            Assert.DoesNotThrow(() =>
            {
                // act
                var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
                Assert.IsNotNull(configReader);
                Assert.IsFalse(configReader.IsConfigLoaded);
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
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);

            // assert
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("foo"), "Rules that dont have a validator should be set to off");
            Assert.IsFalse(configReader.IsConfigLoaded);
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
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);

            // assert
            Assert.AreEqual(RuleViolationSeverity.Off, configReader.GetRuleSeverity("select-star"), "Rules that dont have a valid severity should be set to off");
            Assert.IsTrue(configReader.IsConfigLoaded);
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
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);

            // assert
            reporter.Received().Report("Config file is not valid Json.");
            Assert.IsFalse(configReader.IsConfigLoaded);
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
                { pluginPath, new MockFileData(string.Empty) }
            });

            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);
            var plugins = configReader.GetPlugins();
            configReader.ListPlugins();

            // assert
            Assert.AreEqual(RuleViolationSeverity.Error, configReader.GetRuleSeverity("select-star"));
            Assert.AreEqual(RuleViolationSeverity.Warning, configReader.GetRuleSeverity("statement-semicolon-termination"));
            Assert.IsTrue(configReader.IsConfigLoaded);

            Assert.AreEqual(true, plugins.ContainsKey("my-first-plugin"));
            Assert.AreEqual(true, plugins.ContainsKey("my-second-plugin"));

            Assert.AreEqual("c:/users/someone/my-plugins/my-first-plugin.dll", plugins["my-first-plugin"]);
            Assert.AreEqual("c:/users/someone/my-plugins/my-second-plugin.dll", plugins["my-second-plugin"]);

            reporter.Received().Report("Found the following plugins:");
            reporter.Received().Report("Plugin Name 'my-first-plugin' loaded from path 'c:/users/someone/my-plugins/my-first-plugin.dll'");
            reporter.Received().Report("Plugin Name 'my-second-plugin' loaded from path 'c:/users/someone/my-plugins/my-second-plugin.dll'");
        }
    }
}
