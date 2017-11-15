using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace TSQLLint.Tests.Helpers
{
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

                var workingDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory);
                _ApplicationPath = $@"{workingDirectory.Replace("TSQLLint.Tests", "TSQLLint.Console")}/TSQLLint.Console.dll";

                return _ApplicationPath;
            }
        }

        public static Process GetProcess(string arguments, DataReceivedEventHandler OutputHandler, DataReceivedEventHandler ErrorHandler, EventHandler ExitHandler)
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
