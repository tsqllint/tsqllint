using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Console;
using TSQLLint.Infrastructure.Rules.RuleViolations;
using TSQLLint.Tests.Helpers.ObjectComparers;

namespace TSQLLint.Tests.Helpers
{
    public static class TestHelper
    {
        private static readonly RuleViolationComparer RuleViolationComparer = new RuleViolationComparer();

        public static void PerformApplicationTest(List<string> argumentsUnderTest, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            // arrange
            expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.Line).ToList();

            var appArgs = argumentsUnderTest.ToArray();
            var mockReporter = Substitute.For<IReporter>();

            var reportedViolations = new List<IRuleViolation>();
            mockReporter.When(reporter => reporter.ReportViolation(Arg.Any<IRuleViolation>())).Do(x => reportedViolations.Add(x.Arg<IRuleViolation>()));
            var reportedMessages = new List<string>();
            mockReporter.When(reporter => reporter.ReportFileResults()).Do(x =>
            {
                foreach (var v in reportedViolations)
                {
                    reportedMessages.Add(v.Text);
                }
            });

            mockReporter.When(reporter => reporter.Report(Arg.Any<string>())).Do(x => reportedMessages.Add(x.Arg<string>()));
            var application = new Application(appArgs, mockReporter);

            // act
            application.Run();

            // assert
            Assert.AreEqual(expectedRuleViolations.Count, reportedViolations.Count);
            Assert.AreEqual(expectedRuleViolations.Count, reportedViolations.Count);
            Assert.IsTrue(string.IsNullOrEmpty(expectedMessage) || reportedMessages.Contains(expectedMessage), $"Expected: '{expectedMessage}', Received: '{string.Join(" ", reportedMessages)}'");

            reportedViolations = reportedViolations.OrderBy(o => o.Line).ThenBy(o => o.RuleName).ToList();
            expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.Line).ThenBy(o => o.RuleName).ToList();
            CollectionAssert.AreEqual(expectedRuleViolations, reportedViolations, RuleViolationComparer);
        }
    }
}
