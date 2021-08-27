using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Runtime.InteropServices;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Infrastructure.Plugins;
using TSQLLint.Tests.Helpers;

namespace TSQLLint.Tests.UnitTests.PluginHandler
{
    public class PluginHandlerTests
    {
        [Test]
        public void LoadPlugins_ShouldLoadPluginsFromPathAndFile()
        {
            // arrange
            var filePath1 = TestHelper.GetTestFilePath(@"c:\pluginDirectory\plugin_one.dll");
            var filePath2 = TestHelper.GetTestFilePath(@"c:\pluginDirectory\plugin_two.dll");
            var filePath3 = TestHelper.GetTestFilePath(@"c:\pluginDirectory\plugin_three.dll");
            var filePath4 = TestHelper.GetTestFilePath(@"c:\pluginDirectory\foo.txt");
            var filePath5 = TestHelper.GetTestFilePath(@"c:\pluginDirectory\subDirectory\bar.txt");
            var filePath6 = TestHelper.GetTestFilePath(@"c:\pluginDirectory\subDirectory\plugin_four.dll");

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData(string.Empty)
                },
                {
                    filePath2, new MockFileData(string.Empty)
                },
                {
                    filePath3, new MockFileData(string.Empty)
                },
                {
                    filePath4, new MockFileData("foo")
                },
                {
                    filePath5, new MockFileData("bar")
                },
                {
                    filePath6, new MockFileData(string.Empty)
                },
            });

            var assemblyWrapper = new TestAssemblyWrapper(new Dictionary<string, int>
            {
                { TestHelper.GetTestFilePath(@"c:\pluginDirectory\plugin_one.dll"), 0 },
                { TestHelper.GetTestFilePath(@"c:\pluginDirectory\plugin_two.dll"), 1 },
                { TestHelper.GetTestFilePath(@"c:\pluginDirectory\plugin_three.dll"), 2 },
                { TestHelper.GetTestFilePath(@"c:\pluginDirectory\subDirectory\plugin_four.dll"), 3 }
            });

            var reporter = Substitute.For<IReporter>();

            var versionWrapper = Substitute.For<IFileversionWrapper>();
            versionWrapper.GetVersion(Arg.Any<Assembly>()).Returns("1.2.3");

            var pluginPaths = new Dictionary<string, string>
            {
                {
                    "my-first-plugin", TestHelper.GetTestFilePath(@"c:\pluginDirectory\")
                },
                {
                    "my-second-plugin", filePath6
                }
            };

            // act
            var pluginHandler = new Infrastructure.Plugins.PluginHandler(reporter, fileSystem, assemblyWrapper, versionWrapper);
            pluginHandler.ProcessPaths(pluginPaths);

            // assert
            Assert.AreEqual(4, pluginHandler.Plugins.Count);
        }

        [Test]
        public void LoadPlugins_ShouldLoadPluginsFilesWithRelativePaths()
        {
            // arrange
            var currentDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            var executingLocationParentFolder = currentDirectory.Parent.FullName;

            var filePath1 = Path.Combine(executingLocationParentFolder, "plugin_one.dll");
            var filePath2 = Path.Combine(executingLocationParentFolder, "plugin_two.dll");

            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    {
                        filePath1, new MockFileData(string.Empty)
                    },
                    {
                        filePath2, new MockFileData(string.Empty)
                    }
                },
                currentDirectory.FullName);

            var assemblyWrapper = new TestAssemblyWrapper();

            var reporter = Substitute.For<IReporter>();

            var versionWrapper = Substitute.For<IFileversionWrapper>();
            versionWrapper.GetVersion(Arg.Any<Assembly>()).Returns("1.2.3");

            var pluginPaths = new Dictionary<string, string>
            {
                {
                    "my-second-plugin", @"..\plugin_two.dll"
                }
            };

            // act
            var pluginHandler = new Infrastructure.Plugins.PluginHandler(reporter, fileSystem, assemblyWrapper, versionWrapper);
            pluginHandler.ProcessPaths(pluginPaths);

            // assert
            Assert.AreEqual(1, pluginHandler.Plugins.Count);
        }

        [Test]
        public void LoadPlugins_ShouldLoadPluginsDirectoriesWithRelativePaths()
        {
            // arrange
            var pluginName = "plugin_one.dll";
            var currentDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            var executingLocationParentFolder = currentDirectory.Parent.FullName;

            var filePath1 = Path.Combine(executingLocationParentFolder, "subDirectory", pluginName);

            var fileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData>
                {
                    {
                        filePath1, new MockFileData(string.Empty)
                    }
                },
                currentDirectory.FullName);

            var assemblyWrapper = new TestAssemblyWrapper();

            var reporter = Substitute.For<IReporter>();

            var versionWrapper = Substitute.For<IFileversionWrapper>();
            versionWrapper.GetVersion(Arg.Any<Assembly>()).Returns("1.2.3");

            var pluginPaths = new Dictionary<string, string>
            {
                {
                    pluginName, @"..\subDirectory"
                }
            };

            // act
            var pluginHandler = new Infrastructure.Plugins.PluginHandler(reporter, fileSystem, assemblyWrapper, versionWrapper);
            pluginHandler.ProcessPaths(pluginPaths);

            // assert
            Assert.AreEqual(1, pluginHandler.Plugins.Count);
        }

        [Test]
        public void LoadPlugins_ThrowErrors_When_Same_Type_Is_Loaded_More_Than_Once()
        {
            // arrange
            var filePath1 = TestHelper.GetTestFilePath(@"c:\pluginDirectory\plugin_one.dll");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData(string.Empty)
                }
            });

            var assemblyWrapper = new TestAssemblyWrapper(defaultPlugin: typeof(TestPluginThrowsException));

            var pluginPaths = new Dictionary<string, string>
            {
                {
                    "my-plugin", filePath1
                },
                {
                    "my-plugin-directories", TestHelper.GetTestFilePath(@"c:\pluginDirectory")
                },
                {
                    "my-plugin-invalid-path", TestHelper.GetTestFilePath(@"c:\doesnt-exist")
                }
            };

            var versionWrapper = Substitute.For<IFileversionWrapper>();
            versionWrapper.GetVersion(Arg.Any<Assembly>()).Returns("1.2.3");

            var reporter = Substitute.For<IReporter>();

            // act
            var pluginHandler = new Infrastructure.Plugins.PluginHandler(reporter, fileSystem, assemblyWrapper, versionWrapper);
            pluginHandler.ProcessPaths(pluginPaths);

            // assert
            Assert.AreEqual(1, pluginHandler.Plugins.Count);
            var type = typeof(TestPluginThrowsException);
            reporter.Received().Report($"Loaded plugin: '{type.FullName}', Version: '1.2.3'");
            reporter.Received().Report($"Already loaded plugin with type '{type.FullName}'");
        }

        [Test]
        public void ActivatePlugins_PluginRuleViolations_ShouldCallReporter()
        {
            // arrange
            var filePath1 = TestHelper.GetTestFilePath(@"c:\pluginDirectory\plugin_one.dll");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    filePath1, new MockFileData(string.Empty)
                }
            });

            var assemblyWrapper = new TestAssemblyWrapper();

            var pluginPaths = new Dictionary<string, string>
            {
                {
                    "my-plugin", filePath1
                }
            };

            var reporter = Substitute.For<IReporter>();
            var textReader = ParsingUtility.CreateTextReaderFromString("\tSELECT * FROM FOO");

            var scriptPath = TestHelper.GetTestFilePath(@"c:\scripts\foo.sql");
            var context = new PluginContext(scriptPath, new List<IRuleException>(), textReader);

            var versionWrapper = Substitute.For<IFileversionWrapper>();
            versionWrapper.GetVersion(Arg.Any<Assembly>()).Returns("1.2.3");

            // act
            var pluginHandler = new Infrastructure.Plugins.PluginHandler(reporter, fileSystem, assemblyWrapper, versionWrapper);
            pluginHandler.ProcessPaths(pluginPaths);

            // assert
            Assert.AreEqual(1, pluginHandler.Plugins.Count);
            Assert.DoesNotThrow(() => pluginHandler.ActivatePlugins(context));

            reporter.Received().ReportViolation(Arg.Is<IRuleViolation>(x =>
                x.FileName == context.FilePath
                && x.RuleName == "prefer-tabs"
                && x.Text == "Should use spaces rather than tabs"
                && x.Line == 1
                && x.Column == 0
                && x.Severity == RuleViolationSeverity.Warning));
        }

        [Test]
        public void ActivatePlugins_ThrowErrors_ShouldCatch_ShouldReport()
        {
            // arrange
            var testFilePath = TestHelper.GetTestFilePath(@"UnitTests/PluginHandler/tsqllint-plugin-throws-exception.dll");
            var path = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, testFilePath));
            var assemblyWrapper = new AssemblyWrapper();

            var pluginPaths = new Dictionary<string, string>
            {
                {
                    "my-plugin", path
                }
            };

            var reporter = Substitute.For<IReporter>();
            var versionWrapper = Substitute.For<IFileversionWrapper>();
            var context = Substitute.For<IPluginContext>();
            versionWrapper.GetVersion(Arg.Any<Assembly>()).Returns("1.2.3");

            // act
            var pluginHandler = new Infrastructure.Plugins.PluginHandler(reporter, new FileSystem(), assemblyWrapper, versionWrapper);
            pluginHandler.ProcessPaths(pluginPaths);
            pluginHandler.ActivatePlugins(context);

            // assert
            Assert.AreEqual(1, pluginHandler.Plugins.Count);
            reporter.Received().Report(@"There was a problem with plugin: tsqllint_plugin_throws_exception.PluginThatThrows - something bad happened");
        }

        public class TestPlugin2 : TestPlugin
        {
        }

        public class TestPlugin3 : TestPlugin
        {
        }

        public class TestPlugin4 : TestPlugin
        {
        }

        public class TestPluginThrowsException : IPlugin
        {
            public void PerformAction(IPluginContext context, IReporter reporter)
            {
                throw new NotImplementedException();
            }
        }

        public class TestAssemblyWrapper : IAssemblyWrapper
        {
            private readonly Assembly assembly;
            private readonly Dictionary<string, int> pathsToPluginNumber;
            private string assemblyLoaded;
            private Type defaultPlugin;

            public TestAssemblyWrapper(Dictionary<string, int> pathsToPluginNumber = null, Type defaultPlugin = null)
            {
                assembly = Assembly.GetExecutingAssembly();
                this.pathsToPluginNumber = pathsToPluginNumber;
                this.defaultPlugin = defaultPlugin ?? typeof(TestPlugin);
            }

            public Assembly LoadFile(string path)
            {
                assemblyLoaded = path;
                return assembly;
            }

            public Type[] GetExportedTypes(Assembly assembly)
            {
                if (pathsToPluginNumber == null || !pathsToPluginNumber.ContainsKey(assemblyLoaded))
                {
                    return new[] { defaultPlugin };
                }

                return pathsToPluginNumber[assemblyLoaded] switch
                {
                    0 => new[] { defaultPlugin },
                    1 => new[] { typeof(TestPlugin2) },
                    2 => new[] { typeof(TestPlugin3) },
                    _ => new[] { typeof(TestPlugin4) }
                };
            }
        }
    }
}
