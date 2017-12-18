using System;
using System.Diagnostics;
using TSQLLint.Lib.Reporters.Interfaces;

namespace TSQLLint.Lib.Reporters
{
    public class ConsoleTimer : IConsoleTimer
    {
        private readonly Stopwatch stopWatch = new Stopwatch();

        public void Start()
        {
            stopWatch.Start();
        }

        public TimeSpan Stop()
        {
            stopWatch.Stop();
            return stopWatch.Elapsed;
        }
    }
}
