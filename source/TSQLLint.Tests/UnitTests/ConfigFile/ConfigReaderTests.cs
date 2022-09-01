using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Configuration;
using TSQLLint.Tests.Helpers;
using static System.String;

namespace TSQLLint.Tests.UnitTests.ConfigFile
{
    [TestFixture]
    public class ConfigReaderTests
    {
        [Test]
        public void ConfigReaderInMemoryConfig()
        {
            // arrange
            var fileSystem = new MockFileSystem();
            var reporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(reporter, fileSystem, environmentWrapper);
            configReader.LoadConfig(Empty); // load config from memory
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
            configReader.LoadConfig(TestHelper.GetTestFilePath(@"c:\users\someone\.tsqllintrc"));

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
                    TestHelper.GetTestFilePath(@"C:\Users\User\.tsqllintrc"), new MockFileData(@"
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
            var testConfigFile = TestHelper.GetTestFilePath(@"c:\foo\.tsqllintrc");

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
                        TestHelper.GetTestFilePath(@"C:\Users\User\.tsqllintrc"), new MockFileData(@"
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
            var configFilePath = TestHelper.GetTestFilePath(@"C:\Users\User\.tsqllintrc");
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

        // Test misspelled "compatability-level"
        [Test]
        public void ConfigReaderGetParserFromValidIntOld()
        {
            // arrange
            var configFilePath = TestHelper.GetTestFilePath(@"C:\Users\User\.tsqllintrc");
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"
                    {
                        'compatability-level': 120
                    }")
                }
            });

            var mockReporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(mockReporter, mockFileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);

            // assert
            Assert.IsTrue(configReader.IsConfigLoaded);
            Assert.AreEqual(120, configReader.CompatabilityLevel);
        }

        // Test misspelled "compatability-level" with correctly spelled "compatibility-level"
        [Test]
        public void ConfigReaderGetParserFromValidIntFallback()
        {
            // arrange
            var configFilePath = TestHelper.GetTestFilePath(@"C:\Users\User\.tsqllintrc");
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"
                    {
                        'compatability-level': 120,
                        'compatibility-level': 130
                    }")
                }
            });

            var mockReporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(mockReporter, mockFileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);

            // assert
            Assert.IsTrue(configReader.IsConfigLoaded);
            Assert.AreEqual(130, configReader.CompatabilityLevel);
        }

        [Test]
        public void ConfigReaderGetParserFromValidInt()
        {
            // arrange
            var configFilePath = TestHelper.GetTestFilePath(@"C:\Users\User\.tsqllintrc");
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"
                    {
                        'compatibility-level': 120
                    }")
                }
            });

            var mockReporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(mockReporter, mockFileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);

            // assert
            Assert.IsTrue(configReader.IsConfigLoaded);
            Assert.AreEqual(120, configReader.CompatabilityLevel);
        }

        [Test]
        public void ConfigReaderGetParserFromValidString()
        {
            // arrange
            var configFilePath = TestHelper.GetTestFilePath(@"C:\Users\User\.tsqllintrc");
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"
                    {
                        'compatibility-level': '130'
                    }")
                }
            });

            var mockReporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(mockReporter, mockFileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);

            // assert
            Assert.IsTrue(configReader.IsConfigLoaded);
            Assert.AreEqual(130, configReader.CompatabilityLevel);
        }

        [Test]
        public void ConfigReaderGetParserFromInValidInt()
        {
            // arrange
            var configFilePath = TestHelper.GetTestFilePath(@"C:\Users\User\.tsqllintrc");
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"
                    {
                        'compatibility-level': 10
                    }")
                }
            });

            var mockReporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(mockReporter, mockFileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);

            // assert
            Assert.IsTrue(configReader.IsConfigLoaded);
            Assert.AreEqual(120, configReader.CompatabilityLevel);
        }

        [Test]
        public void ConfigReaderGetParserFromInValidString()
        {
            // arrange
            var configFilePath = TestHelper.GetTestFilePath(@"C:\Users\User\.tsqllintrc");
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"
                    {
                        'compatibility-level': 'foo'
                    }")
                }
            });

            var mockReporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(mockReporter, mockFileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);

            // assert
            Assert.IsTrue(configReader.IsConfigLoaded);
            Assert.AreEqual(120, configReader.CompatabilityLevel);
        }

        [Test]
        public void ConfigReaderGetParserNotSet()
        {
            // arrange
            var configFilePath = TestHelper.GetTestFilePath(@"C:\Users\User\.tsqllintrc");
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    configFilePath, new MockFileData(@"
                    {
                    }")
                }
            });

            var mockReporter = Substitute.For<IReporter>();
            var environmentWrapper = Substitute.For<IEnvironmentWrapper>();

            // act
            var configReader = new ConfigReader(mockReporter, mockFileSystem, environmentWrapper);
            configReader.LoadConfig(configFilePath);

            // assert
            Assert.IsTrue(configReader.IsConfigLoaded);
            Assert.AreEqual(120, configReader.CompatabilityLevel);
        }

        [Test]
        public void ConfigReaderNoRulesNoThrow()
        {
            // arrange
            var configFilePath = TestHelper.GetTestFilePath(@"c:\users\someone\.tsqllintrc-missing-rules");
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
            var configFilePath = TestHelper.GetTestFilePath(@"c:\users\someone\.tsqllintrc");
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
            var configFilePath = TestHelper.GetTestFilePath(@"c:\users\someone\.tsqllintrc-bad-severity");
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
            var configFilePath = TestHelper.GetTestFilePath(@"c:\users\someone\.tsqllintrc-bad-json");
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
            var configFilePath = TestHelper.GetTestFilePath(@"c:\users\someone\.tsqllintrc");
            var pluginPath = TestHelper.GetTestFilePath(@"c:\users\someone\my-plugins\foo.dll");

            var plugOnePath = TestHelper.GetTestFilePath(@"c:/users/someone/my-plugins/my-first-plugin.dll");
            var plugTwoPath = TestHelper.GetTestFilePath(@"c:/users/someone/my-plugins/my-second-plugin.dll");

            var fileContent = $@"{{
               ""rules"": {{
               ""select-star"": ""error"",
                   ""statement-semicolon-termination"": ""warning""
               }},
               ""plugins"": {{
                   ""my-first-plugin"": ""{plugOnePath}"",
                   ""my-second-plugin"": ""{plugTwoPath}""
               }}
            }}";

           var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
           {
               {
                   configFilePath, new MockFileData(fileContent)
               },
               { pluginPath, new MockFileData(Empty) }
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

           Assert.AreEqual(TestHelper.GetTestFilePath("c:/users/someone/my-plugins/my-first-plugin.dll"), plugins["my-first-plugin"]);
           Assert.AreEqual(TestHelper.GetTestFilePath("c:/users/someone/my-plugins/my-second-plugin.dll"), plugins["my-second-plugin"]);

           reporter.Received().Report("Found the following plugins:");
           reporter.Received().Report($@"Plugin Name 'my-first-plugin' loaded from path '{plugOnePath}'");
           reporter.Received().Report($@"Plugin Name 'my-second-plugin' loaded from path '{plugTwoPath}'");
        }
    }
}
