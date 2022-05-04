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
                var fileLineActions = new FileLineActions(fileViolations, fileLines);

                foreach (var violation in fileViolations)
                {
                    if (Rules.ContainsKey(violation.RuleName))
                    {
                        if (violation.Line == 1 && violation.Column > fileLines[violation.Line].Length)
                        {
                            // https://github.com/tsqllint/tsqllint/issues/294
                            // There is a pretty bad bug with dynamic sql that I can't figure out.
                            // If you use SET instead of SELECT
                            // DECLARE @Sql NVARCHAR(4000);
                            // SET @Sql = 'CREATE PROCEDURE dbo.test AS RETURN 0';
                            // EXEC(@Sql);
                            // Then all the dynamic sql error happen to line 1.
                            // This is a super hacky way around the issue.
                            // ALSO if you change the SET to SELECT,
                            // then the dynamic sql no longer validates any rules.
                            continue;
                        }

                        var lines = new List<string>(fileLines);
                        Rules[violation.RuleName].FixViolation(lines, violation, fileLineActions);
                    }
                }

                FileSystem.File.WriteAllLines(file.Key, fileLines);
            }
        }
    }
}
