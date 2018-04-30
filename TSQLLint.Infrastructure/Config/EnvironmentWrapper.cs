using System;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Config
{
    public class EnvironmentWrapper : IEnvironmentWrapper
    {
        public string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}
