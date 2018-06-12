using System;
using System.Diagnostics;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Reporters
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
