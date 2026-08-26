using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Infrastructure.Interfaces;

namespace TSQLLint.Infrastructure.Parser
{
    public class ViolationFixer : IViolationFixer
    {
        private readonly IFileSystem FileSystem;
        private readonly Dictionary<string, ISqlLintRule> Rules;
        private readonly IList<IRuleViolation> Violations;
        private readonly IReporter Reporter;

        public ViolationFixer(
            IFileSystem fileSystem,
            Dictionary<string, ISqlLintRule> rules,
            IList<IRuleViolation> violations,
            IReporter reporter = null)
        {
            Rules = rules;
            FileSystem = fileSystem;
            Violations = violations;
            Reporter = reporter;
        }

        public ViolationFixer(
            IFileSystem fileSystem,
            IList<IRuleViolation> violations,
            IReporter reporter = null)
            : this(fileSystem, RuleVisitorFriendlyNameTypeMap.Rules, violations, reporter)
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
                        if (violation.Line == 1 && violation.Column > fileLines[violation.Line - 1].Length + 1)
                        {
                            // https://github.com/tsqllint/tsqllint/issues/294
                            // There is a pretty bad bug with dynamic sql that I can't figure out.
                            // If you use SET instead of SELECT
                            // DECLARE @Sql NVARCHAR(4000);
                            // SET @Sql = 'CREATE PROCEDURE dbo.test AS RETURN 0';
                            // EXEC(@Sql);
                            // Then all the dynamic sql error happen to line 1.
                            // This is a super hacky way around the issue that will usually work.
                            // ALSO if you change the SET to SELECT,
                            // then the dynamic sql no longer validates any rules.
                            continue;
                        }

                        var lines = new List<string>(fileLines);
                        try
                        {
                            Rules[violation.RuleName].FixViolation(lines, violation, fileLineActions);
                        }
                        catch (Exception ex)
                        {
                            // A fixer re-parses the file, and some constructs the linter tolerates
                            // cannot be re-parsed by the fix helpers (e.g. CREATE OR ALTER, see
                            // https://github.com/tsqllint/tsqllint/issues/337). Skip the offending
                            // fix instead of aborting every remaining fix in the run.
                            Reporter?.Report(
                                $"{file.Key}({violation.Line},{violation.Column}): unable to fix {violation.RuleName} : {ex.Message}");
                        }
                    }
                }

                FileSystem.File.WriteAllLines(file.Key, fileLines);
            }
        }
    }
}
