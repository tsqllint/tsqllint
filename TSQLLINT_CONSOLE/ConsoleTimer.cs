using System;
using System.Diagnostics;

namespace TSQLLINT_CONSOLE
{
    public class ConsoleTimer
    {
        private readonly Stopwatch _stopWatch = new Stopwatch();

        public void Start()
        {
            _stopWatch.Start();
        }

        public TimeSpan Stop()
        {
            _stopWatch.Stop();
            return _stopWatch.Elapsed; 
        }
    }
}