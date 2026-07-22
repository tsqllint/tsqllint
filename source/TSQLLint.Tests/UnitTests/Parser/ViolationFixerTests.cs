using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Parser
{
    [TestFixture]
    public class ViolationFixerTests
    {
        // issue #337: a fixer that re-parses the file can throw on constructs the linter
        // tolerates (e.g. CREATE OR ALTER). One failing fix must not abort the whole run.
        [Test]
        public void Fix_WhenRuleFixerThrows_SkipsViolationAndContinues()
        {
            const string filePath = @"c:\history.sql";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { filePath, new MockFileData("line-a\r\nline-b\r\n") }
            });

            var throwingRule = Substitute.For<ISqlLintRule>();
            throwingRule
                .When(x => x.FixViolation(Arg.Any<List<string>>(), Arg.Any<IRuleViolation>(), Arg.Any<FileLineActions>()))
                .Do(_ => throw new Exception("Parsing failed. Incorrect syntax near CREATE.. Incorrect syntax near OR."));

            var healthyRule = Substitute.For<ISqlLintRule>();

            var rules = new Dictionary<string, ISqlLintRule>
            {
                { "throwing-rule", throwingRule },
                { "healthy-rule", healthyRule }
            };

            // Violations are processed bottom-of-file first (OrderByDescending line/column),
            // so putting the throwing rule on the later line guarantees it runs before the
            // healthy rule -- proving the run continues past the failure rather than the
            // healthy fix merely having already happened.
            var violations = new List<IRuleViolation>
            {
                new RuleViolation(filePath, "healthy-rule", 1, 1),
                new RuleViolation(filePath, "throwing-rule", 2, 1)
            };

            var reporter = Substitute.For<IReporter>();

            var fixer = new ViolationFixer(fileSystem, rules, violations, reporter);

            // the throwing fixer must not take down the whole run
            Assert.DoesNotThrow(() => fixer.Fix());

            // the healthy fixer still ran even though the other one threw
            healthyRule.Received().FixViolation(Arg.Any<List<string>>(), Arg.Any<IRuleViolation>(), Arg.Any<FileLineActions>());

            // and the user was told which fix was skipped
            reporter.Received().Report(Arg.Is<string>(m => m.Contains("throwing-rule")));
        }
    }
}
