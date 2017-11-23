using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Console;
using TSQLLint.Console.CommandLineOptions;
using TSQLLint.Lib.Rules.RuleViolations;
using TSQLLint.Tests.Helpers;

namespace TSQLLint.Tests.IntegrationTests
{
    public class IntegrationBaseTest
    {
        protected readonly string DefaultConfigFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tsqllintrc");

        protected static string TestFileDirectory => Path.Combine(TestContext.CurrentContext.WorkDirectory, @"IntegrationTests/Configuration/TestFiles");

        protected static string TestFileOne => Path.Combine(TestFileDirectory, @"integration-test-one.sql");

        protected static string UsageString => new CommandLineOptions(new string[]{}).GetUsage();

        protected static string TSqllVersion
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        protected static readonly IEnumerable<RuleViolation> TestFileOneRuleViolations = new List<RuleViolation>
        {
            new RuleViolation("conditional-begin-end", 2, 1),
            new RuleViolation("data-compression", 6, 1),
            new RuleViolation("data-type-length", 13, 16),
            new RuleViolation("disallow-cursors", 17, 1),
            new RuleViolation("information-schema", 20, 27),
            new RuleViolation("non-sargable", 38, 7),
            new RuleViolation("keyword-capitalization", 23, 1),
            new RuleViolation("multi-table-alias", 27, 10),
            new RuleViolation("object-property", 38, 7),
            new RuleViolation("print-statement", 42, 1),
            new RuleViolation("schema-qualify", 45, 17),
            new RuleViolation("select-star", 48, 8),
            new RuleViolation("semicolon-termination", 51, 31),
            new RuleViolation("set-ansi", 1, 1),
            new RuleViolation("set-nocount", 1, 1),
            new RuleViolation("set-quoted-identifier", 1, 1),
            new RuleViolation("set-transaction-isolation-level", 1, 1),
            new RuleViolation("upper-lower", 59, 8)
        };

        protected static readonly IEnumerable<RuleViolation> TestFileInvalidSyntaxRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(null, null, "TSQL not syntactically correct", 0, 0, RuleViolationSeverity.Error)
        };

        protected static readonly IEnumerable<RuleViolation> TestFileTwoRuleViolations = new List<RuleViolation>
        {
            new RuleViolation("print-statement", 5, 1)
        };

        private readonly RuleViolationComparer _ruleViolationComparer = new RuleViolationComparer();

        protected void PerformApplicationTest(List<string> argumentsUnderTest, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            // arrange
            expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.Line).ToList();

            var appArgs = argumentsUnderTest.ToArray();
            var mockReporter = Substitute.For<IReporter>();

            var reportedViolations = new List<IRuleViolation>();
            mockReporter.When(reporter => reporter.ReportViolation(Arg.Any<IRuleViolation>())).Do(x => reportedViolations.Add(x.Arg<IRuleViolation>()));

            var reportedMessages = new List<string>();
            mockReporter.When(reporter => reporter.Report(Arg.Any<string>())).Do(x => reportedMessages.Add(x.Arg<string>()));

            var application = new Application(appArgs, mockReporter);

            // act
            application.Run();

            // assert
            Assert.AreEqual(expectedRuleViolations.Count, reportedViolations.Count);
            reportedViolations = reportedViolations.OrderBy(o => o.Line).ToList();
            Assert.IsTrue(string.IsNullOrEmpty(expectedMessage) || reportedMessages.Contains(expectedMessage), $"Expected: '{expectedMessage}', Received: '{string.Join(" ", reportedMessages)}'");
            CollectionAssert.AreEqual(expectedRuleViolations, reportedViolations, _ruleViolationComparer);
        }
    }
}
