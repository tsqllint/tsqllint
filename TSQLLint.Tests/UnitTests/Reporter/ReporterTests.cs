using System;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Lib.Reporters;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Tests.UnitTests.Reporter
{
    [ExcludeFromCodeCoverage]
    public class ReporterTests
    {
        [Test]
        public void ConsoleReporter_ReportResults()
        {
            // arrange
            var reporter = Substitute.ForPartsOf<ConsoleReporter>();
            reporter.When(x => x.Report(Arg.Any<string>())).DoNotCallBase(); // suppress console output

            // act
            reporter.ReportViolation(new RuleViolation("foo.sql", "rule name", "rule text", 1, 1, RuleViolationSeverity.Error));
            reporter.ReportViolation(new RuleViolation("foo.sql", "rule name", "rule text", 1, 1, RuleViolationSeverity.Warning));
            reporter.ReportViolation(new RuleViolation("foo.sql", "rule name", "rule text", 1, 1, RuleViolationSeverity.Off)); // should not log
            reporter.ReportResults(new TimeSpan(1, 1, 1), 3);

            // assert
            reporter.Received().Report("foo.sql(1,1): error rule name : rule text.");
            reporter.Received().Report("foo.sql(1,1): warning rule name : rule text.");
            reporter.DidNotReceive().Report("foo.sql(1,1): off rule name : rule text.");
            reporter.Received().Report("\nLinted 3 files in 3661 seconds\n\n1 Errors.\n1 Warnings");

            Assert.Throws<NotImplementedException>(() =>
            {
                reporter.ReportFileResults();
            });
        }
    }
}
