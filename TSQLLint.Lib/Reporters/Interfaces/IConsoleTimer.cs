using System;

namespace TSQLLint.Lib.Standard.Reporters.Interfaces
{
    public interface IConsoleTimer
    {
        void Start();
        
        TimeSpan Stop();
    }
}
