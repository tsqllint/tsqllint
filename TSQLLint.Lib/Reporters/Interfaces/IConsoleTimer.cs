using System;

namespace TSQLLint.Console.Reporters.Interfaces
{
    public interface IConsoleTimer
    {
        void Start();
        
        TimeSpan Stop();
    }
}
