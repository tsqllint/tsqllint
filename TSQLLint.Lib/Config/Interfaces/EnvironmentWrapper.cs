using System;

namespace TSQLLint.Lib.Config.Interfaces
{
    public class EnvironmentWrapper : IEnvironmentWrapper
    {
        public string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}
