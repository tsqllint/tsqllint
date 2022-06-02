using System.Collections.Generic;
using TSQLLint.Common;

namespace TSQLLint.Infrastructure.Reporters
{
    public interface IConsoleReporter : IReporter
    {
        int? FixedCount { get; set; }
        bool ReporterMuted { get; set; }
        public bool ShouldCollectViolations { get; set; }
        public List<IRuleViolation> Violations { get; }

        void ClearViolations();
    }
}
