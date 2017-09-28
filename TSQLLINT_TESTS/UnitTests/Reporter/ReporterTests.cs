﻿using System;
using NSubstitute;
using NUnit.Framework;
using TSQLLINT_COMMON;
using TSQLLINT_CONSOLE.Reporters;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB_TESTS.UnitTests.Reporter
{
    public class ReporterTests
    {
        [Test]
        public void ConsoleReporter_ReportResults()
        {
            //arrange
            var reporter = Substitute.ForPartsOf<ConsoleReporter>();
            reporter.When(x => x.Report(Arg.Any<string>())).DoNotCallBase(); // suppress console output

            //act
            reporter.ReportViolation(new RuleViolation("foo.sql", "rule name", "rule text", 1, 1, RuleViolationSeverity.Error ));
            reporter.ReportViolation(new RuleViolation("foo.sql", "rule name", "rule text", 1, 1, RuleViolationSeverity.Warning ));
            reporter.ReportViolation(new RuleViolation("foo.sql", "rule name", "rule text", 1, 1, RuleViolationSeverity.Off )); // should not log
            reporter.ReportResults(new TimeSpan(1, 1, 1), 3);

            //assert
            reporter.Received().Report("foo.sql(1,1): error rule name : rule text.");
            reporter.Received().Report("foo.sql(1,1): warning rule name : rule text.");
            reporter.Received().Report("foo.sql(1,1): off rule name : rule text.");
            reporter.Received().Report("\nLinted 3 files in 3661 seconds\n\n1 Errors.\n1 Warnings");
        }

        [Test]
        public void ConsoleReporter_InvalidSeverity_ShouldThrow()
        {
            //arrange
            var reporter = Substitute.ForPartsOf<ConsoleReporter>();
            reporter.When(x => x.Report(Arg.Any<string>())).DoNotCallBase(); // suppress console output

            RuleViolationSeverity invalidSeverity = (RuleViolationSeverity) 99;

            //assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                //act
                reporter.ReportViolation(new RuleViolation("foo.sql", "rule name", "rule text", 1, 1, invalidSeverity));
            });
        }
    }
}