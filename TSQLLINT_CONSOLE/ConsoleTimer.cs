using System;
using System.Diagnostics;

namespace TSQLLINT_CONSOLE
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