using System;
using System.Diagnostics;

namespace TSQLLint.Console.Reporters
{
    public class ConsoleTimer
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
