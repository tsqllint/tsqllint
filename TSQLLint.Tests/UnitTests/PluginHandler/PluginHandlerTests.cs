using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Parser.Interfaces;
using TSQLLint.Lib.Plugins;

namespace TSQLLint.Tests.UnitTests.PluginHandler
{
    public class PluginHandlerTests
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
        public void LoadPlugins_ShouldLoadPluginsFromPathAndFile()
        {
            // arrange
            const string filePath1 = @"c:\pluginDirectory\plugin_one.dll";
            const string filePath2 = @"c:\pluginDirectory\plugin_two.dll";
            const string filePath3 = @"c:\pluginDirectory\plugin_three.dll";
            const string filePath4 = @"c:\pluginDirectory\foo.txt";
            const string filePath5 = @"c:\pluginDirectory\subDirectory\bar.txt";
            const string filePath6 = @"c:\pluginDirectory\subDirectory\plugin_four.dll";

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
                { @"c:\pluginDirectory\plugin_one.dll", 0 },
                { @"c:\pluginDirectory\plugin_two.dll", 1 },
                { @"c:\pluginDirectory\plugin_three.dll", 2 },
                { @"c:\pluginDirectory\subDirectory\plugin_four.dll", 3 }
            });

            var reporter = Substitute.For<IReporter>();

            var pluginPaths = new Dictionary<string, string>
            {
                {
                    "my-first-plugin", @"c:\pluginDirectory\"
                },
                {
                    "my-second-plugin", filePath6
                }
            };

            // act
            var pluginHandler = new Lib.Plugins.PluginHandler(reporter, fileSystem, assemblyWrapper);
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

            var pluginPaths = new Dictionary<string, string>
            {
                {
                    "my-second-plugin", @"..\plugin_one.dll"
                }
            };

            // act
            var pluginHandler = new Lib.Plugins.PluginHandler(reporter, fileSystem, assemblyWrapper);
            pluginHandler.ProcessPaths(pluginPaths);

            // assert
            Assert.AreEqual(1, pluginHandler.Plugins.Count);
        }

        [Test]
        public void LoadPlugins_ShouldLoadPluginsDirectoriesWithRelativePaths()
        {
            // arrange
            var currentDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location);
            var executingLocationParentFolder = currentDirectory.Parent.FullName;

            var filePath1 = Path.Combine(executingLocationParentFolder, "subDirectory", "plugin_one.dll");

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

            var pluginPaths = new Dictionary<string, string>
            {
                {
                    "my-first-plugin", @"..\subDirectory\"
                }
            };

            // act
            var pluginHandler = new Lib.Plugins.PluginHandler(reporter, fileSystem, assemblyWrapper);
            pluginHandler.ProcessPaths(pluginPaths);

            // assert
            Assert.AreEqual(1, pluginHandler.Plugins.Count);
        }

        [Test]
        public void LoadPlugins_ThrowErrors_When_Same_Type_Is_Loaded_More_Than_Once()
        {
            // arrange
            const string filePath1 = @"c:\pluginDirectory\plugin_one.dll";
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
                    "my-plugin-directories", @"c:\pluginDirectory"
                },
                {
                    "my-plugin-invalid-path", @"c:\doesnt-exist"
                }
            };

            var reporter = Substitute.For<IReporter>();

            // act
            var pluginHandler = new Lib.Plugins.PluginHandler(reporter, fileSystem, assemblyWrapper);
            pluginHandler.ProcessPaths(pluginPaths);

            // assert
            Assert.AreEqual(1, pluginHandler.Plugins.Count);
            var type = typeof(TestPluginThrowsException);
            reporter.Received().Report($"Loaded plugin: '{type.FullName}', Version: '1.10.1.0'");
            reporter.Received().Report($"Already loaded plugin with type '{type.FullName}'");
        }

        [Test]
        public void ActivatePlugins_PluginRuleViolations_ShouldCallReporter()
        {
            // arrange
            const string filePath1 = @"c:\pluginDirectory\plugin_one.dll";
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
            var textReader = Lib.Utility.ParsingUtility.CreateTextReaderFromString("\tSELECT * FROM FOO");
            var context = new PluginContext(@"c:\scripts\foo.sql", new List<IRuleException>(), textReader);

            // act
            var pluginHandler = new Lib.Plugins.PluginHandler(reporter, fileSystem, assemblyWrapper);
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
            const string filePath1 = @"c:\pluginDirectory\plugin_one.dll";
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
                    "my-plugin-directories", @"c:\pluginDirectory"
                },
                {
                    "my-plugin-invalid-path", @"c:\doesnt-exist"
                }
            };

            var reporter = Substitute.For<IReporter>();
            var context = Substitute.For<IPluginContext>();

            // act
            var pluginHandler = new Lib.Plugins.PluginHandler(reporter, fileSystem, assemblyWrapper);
            pluginHandler.ProcessPaths(pluginPaths);

            // assert
            Assert.AreEqual(1, pluginHandler.Plugins.Count);
            Assert.Throws<NotImplementedException>(() => pluginHandler.ActivatePlugins(context));
            reporter.Received().Report(Arg.Any<string>());
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

                switch (pathsToPluginNumber[assemblyLoaded])
                {
                    case 0:
                        return new[] { defaultPlugin };
                    case 1:
                        return new[] { typeof(TestPlugin2) };
                    case 2:
                        return new[] { typeof(TestPlugin3) };
                    default:
                        return new[] { typeof(TestPlugin4) };
                }
            }
        }
    }
}
