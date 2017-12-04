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
        private static string _ApplicationPath;

        private static string ApplicationPath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_ApplicationPath))
                {
                    return _ApplicationPath;
                }

                var file = "TSQLLint.Console.exe";
                #if NETCOREAPP2_0
                file = "TSQLLint.Console.dll";
                #endif

                var workingDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory);
                _ApplicationPath = $@"{workingDirectory.Replace("TSQLLint.Tests", "TSQLLint.Console")}/{file}";

                return _ApplicationPath;
            }
        }

        private static string _TestPluginPath;

        public static string TestPluginPath
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_TestPluginPath))
                {
                    return _TestPluginPath;
                }

                var workingDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory);
                _TestPluginPath = $@"{workingDirectory}/TSQLLint.Tests.dll";
                return _TestPluginPath;
            }
        }

        public static Process GetProcess(string arguments, DataReceivedEventHandler OutputHandler, DataReceivedEventHandler ErrorHandler, EventHandler ExitHandler)
        {

            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
#if NET452
                    FileName = ApplicationPath,
                    Arguments = arguments,
#elif NETCOREAPP2_0
                    FileName = "dotnet",
                    Arguments = $"{ApplicationPath} {arguments}",
#endif
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += OutputHandler;
            process.ErrorDataReceived += ErrorHandler;
            process.Exited += ExitHandler;
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
