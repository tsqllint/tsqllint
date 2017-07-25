using System;
using System.Diagnostics;

namespace TSQLLINT_CONSOLE
{
    public class ConsoleTimer
    {
        private readonly Stopwatch StopWatch = new Stopwatch();

        public void start()
        {
            StopWatch.Start();
        }

        public TimeSpan stop()
        {
            StopWatch.Stop();
            return StopWatch.Elapsed; 
        }
    }
}