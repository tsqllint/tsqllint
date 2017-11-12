using System;

namespace TSQLLint.Lib.Reporters.Interfaces
{
    public interface IConsoleTimer
    {
        void Start();
        
        TimeSpan Stop();
    }
}
