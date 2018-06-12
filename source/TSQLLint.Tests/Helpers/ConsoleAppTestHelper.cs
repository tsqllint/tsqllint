using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;

namespace TSQLLint.Tests.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class ConsoleAppTestHelper
    {
        private static string applicationPath;

        private static string testPluginPath;

        public static string TestPluginPath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(testPluginPath))
                {
                    return testPluginPath;
                }

                var workingDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory);
                testPluginPath = $@"{workingDirectory}/TSQLLint.Tests.dll";
                return testPluginPath;
            }
        }

        private static string ApplicationPath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(applicationPath))
                {
                    return applicationPath;
                }

                var workingDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory);
                applicationPath = $@"{workingDirectory.Replace("TSQLLint.Tests", "TSQLLint.Console")}/TSQLLint.Console.dll";

                return applicationPath;
            }
        }

        public static Process GetProcess(string arguments, DataReceivedEventHandler outputHandler, DataReceivedEventHandler errorHandler, EventHandler exitHandler)
        {
            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"{ApplicationPath} {arguments}",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += outputHandler;
            process.ErrorDataReceived += errorHandler;
            process.Exited += exitHandler;
            return process;
        }

        public static void RunApplication(Process process)
        {
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }
    }
}
