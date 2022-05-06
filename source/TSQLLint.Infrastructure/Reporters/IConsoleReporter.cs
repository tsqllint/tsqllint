using System.Collections.Generic;
using TSQLLint.Common;

namespace TSQLLint.Infrastructure.Reporters
{
    public interface IConsoleReporter : IReporter
    {
        public bool ShouldCollectViolations { get; set; }
        public List<IRuleViolation> Violations { get; }
    }
}
