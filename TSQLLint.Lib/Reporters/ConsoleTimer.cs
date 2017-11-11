using System;
using System.Diagnostics;
using TSQLLint.Console.Reporters.Interfaces;

namespace TSQLLint.Console.Reporters
{
    public class ConsoleTimer : IConsoleTimer
    {
        private readonly Stopwatch StopWatch = new Stopwatch();

        public void Start()
        {
            StopWatch.Start();
        }

        public TimeSpan Stop()
        {
            StopWatch.Stop();
            return StopWatch.Elapsed; 
        }
    }
}
