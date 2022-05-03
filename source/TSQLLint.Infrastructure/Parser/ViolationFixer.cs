using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Infrastructure.Interfaces;

namespace TSQLLint.Infrastructure.Parser
{
    public class ViolationFixer : IViolationFixer
    {
        private readonly Dictionary<string, ISqlLintRule> Rules;
        private readonly IFileSystem FileSystem;
        private readonly IList<IRuleViolation> Violations;

        public ViolationFixer(
            IFileSystem fileSystem,
            Dictionary<string, ISqlLintRule> rules,
            IList<IRuleViolation> violations)
        {
            Rules = rules;
            FileSystem = fileSystem;
            Violations = violations;
        }

        public ViolationFixer(
            IFileSystem fileSystem,
            IList<IRuleViolation> violations)
            : this(fileSystem, RuleVisitorFriendlyNameTypeMap.Rules, violations)
        {
        }

        public void Fix()
        {
            var files = Violations.GroupBy(x => x.FileName);

            foreach (var file in files)
            {
                var fileViolations = file
                    .OrderByDescending(x => x.Line)
                    .ThenByDescending(x => x.Column)
                    .ToList();

                var fileLines = FileSystem.File.ReadAllLines(file.Key).ToList();

                foreach (var violation in fileViolations)
                {
                    if (Rules.ContainsKey(violation.RuleName))
                    {
                        Rules[violation.RuleName].FixViolation(fileLines, violation);
                    }
                }

                FileSystem.File.WriteAllLines(file.Key, fileLines);
            }
        }
    }
}
