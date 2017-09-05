﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TSQLLINT_CONSOLE;
using TSQLLINT_LIB.Parser.Interfaces;
using TSQLLINT_LIB.Rules.RuleViolations;
using TSQLLINT_LIB_TESTS.Helpers;

namespace TSQLLINT_LIB_TESTS.IntegrationTests
{
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Needed because of Static property ordering.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Needed because of Static property ordering.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Reviewed. Needed because of Static property ordering.")]
    public class IntegrationMissingConfigTests
    {
        protected readonly string DefaultConfigFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".tsqllintrc");

        [OneTimeTearDown]
        public void TearDown()
        {
            File.Delete(DefaultConfigFile);
        }

        protected static readonly string TestFileDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\IntegrationTests\TestFiles");
        protected static readonly string TestFileOne = Path.Combine(TestFileDirectory, @"integration-test-one.sql");

        protected static readonly IEnumerable<RuleViolation> TestFileOneRuleViolations = new List<RuleViolation>
        {
            new RuleViolation(ruleName: "conditional-begin-end", startLine: 2, startColumn: 1),
            new RuleViolation(ruleName: "data-compression", startLine: 6, startColumn: 1),
            new RuleViolation(ruleName: "data-type-length", startLine: 13, startColumn: 16),
            new RuleViolation(ruleName: "disallow-cursors", startLine: 17, startColumn: 1),
            new RuleViolation(ruleName: "information-schema", startLine: 20, startColumn: 27),
            new RuleViolation(ruleName: "keyword-capitalization", startLine: 23, startColumn: 1),
            new RuleViolation(ruleName: "multi-table-alias", startLine: 27, startColumn: 10),
            new RuleViolation(ruleName: "object-property", startLine: 38, startColumn: 7),
            new RuleViolation(ruleName: "print-statement", startLine: 42, startColumn: 1),
            new RuleViolation(ruleName: "schema-qualify", startLine: 45, startColumn: 17),
            new RuleViolation(ruleName: "select-star", startLine: 48, startColumn: 8),
            new RuleViolation(ruleName: "semicolon-termination", startLine: 51, startColumn: 31),
            new RuleViolation(ruleName: "set-ansi", startLine: 1, startColumn: 1),
            new RuleViolation(ruleName: "set-nocount", startLine: 1, startColumn: 1),
            new RuleViolation(ruleName: "set-quoted-identifier", startLine: 1, startColumn: 1),
            new RuleViolation(ruleName: "set-transaction-isolation-level", startLine: 1, startColumn: 1),
            new RuleViolation(ruleName: "upper-lower", startLine: 59, startColumn: 8),
        };

        private readonly RuleViolationCompare _comparer = new RuleViolationCompare();

        public static IEnumerable MissingConfigTestCases
        {
            get
            {
                yield return new TestCaseData(
                        new List<string> { "-i", TestFileOne },
                        null,
                        TestFileOneRuleViolations,
                        1)
                    .SetName("Init Args Valid Missing Config File");
            }
        }

        [TestCaseSource("MissingConfigTestCases")]
        public void RunMissingConfigTest(List<string> args, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            PerformTest(args, expectedMessage, expectedRuleViolations, expectedFileCount);
        }

        protected void PerformTest(List<string> args, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            // arrange
            var appArgs = args.ToArray();
            var testReporter = new TestReporter();
            var application = new Application(appArgs, testReporter);

            // act
            application.Run();

            // assert
            Assert.IsTrue(string.IsNullOrEmpty(expectedMessage) || testReporter.Messages.Contains(expectedMessage));

            expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.Line).ToList();
            var reportedRuleViolations = testReporter.RuleViolations.OrderBy(o => o.Line).ToList();
            Assert.AreEqual(expectedRuleViolations.Count, expectedRuleViolations.Count);
            CollectionAssert.AreEqual(expectedRuleViolations, reportedRuleViolations, _comparer);

            Assert.AreEqual(expectedFileCount, testReporter.FileCount);
        }
        protected class TestReporter : IReporter
        {
            public TestReporter()
            {
                RuleViolations = new List<RuleViolation>();
                Messages = new List<string>();
            }

            public List<string> Messages { get; private set; }
            public List<RuleViolation> RuleViolations { get; private set; }
            public int FileCount { get; private set; }

            public void ReportResults(TimeSpan timespan, int fileCount)
            {
                FileCount = fileCount;
            }

            public void Report(string message)
            {
                Messages.Add(message);
            }

            public void ReportViolation(RuleViolation violation)
            {
                RuleViolations.Add(violation);
            }
        }
    }
}