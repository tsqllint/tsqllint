using System;
using System.Collections.Concurrent;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Interfaces;

namespace TSQLLint.Infrastructure.Parser
{
    public class ViolationFixer : IViolationFixer
    {
        private readonly bool shouldFix;
        private readonly ConcurrentBag<IRuleViolation> violations = new ConcurrentBag<IRuleViolation>();
        private readonly IFileSystem fileSystem;

        public ViolationFixer(IFileSystem fileSystem, bool shouldFix)
        {
            this.fileSystem = fileSystem;
            this.shouldFix = shouldFix;
        }

        public void AddViolation(IRuleViolation violation)
        {
            if (shouldFix)
            {
                violations.Add(violation);
            }
        }

        public void FixViolations()
        {
            var rules = new ConcurrentDictionary<string, ISqlRule>();
            var files = violations.GroupBy(x => x.FileName);

            foreach(var file in files)
            {
                var fileViolations = file
                    .OrderByDescending(x => x.Line)
                    .ThenByDescending(x => x.Column)
                    .ToList();

                var fileLines = fileSystem.File.ReadAllLines(file.Key);

                foreach (var violation in fileViolations)
                {
                    var rule = rules.GetOrAdd(violation.RuleName, (ruleName)
                        => (ISqlRule)Activator.CreateInstance(RuleVisitorFriendlyNameTypeMap.List[ruleName], (Action<string, string, int, int>)null));

                    rule.FixViolation(fileLines, violation);
                }

                fileSystem.File.WriteAllLines(file.Key, fileLines);
            }
        }
    }
}
