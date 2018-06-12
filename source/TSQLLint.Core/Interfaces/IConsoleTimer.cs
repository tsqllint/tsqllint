using System;

namespace TSQLLint.Core.Interfaces
{
    public interface IConsoleTimer
    {
        void Start();

        TimeSpan Stop();
    }
}
