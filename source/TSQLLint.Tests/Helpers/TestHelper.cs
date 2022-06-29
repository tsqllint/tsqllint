using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NSubstitute;
using NUnit.Framework;
using TSQLLint.Common;
using TSQLLint.Infrastructure.Reporters;
using TSQLLint.Infrastructure.Rules.RuleViolations;
using TSQLLint.Tests.Helpers.ObjectComparers;

namespace TSQLLint.Tests.Helpers
{
    public static class TestHelper
    {
        private static readonly RuleViolationComparer RuleViolationComparer = new RuleViolationComparer();
        private static readonly bool IsExecutingOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static string GetTestFilePath(string path)
        {
            if (!IsExecutingOnWindows)
            {
                if (path.Contains(":"))
                {
                    var splitPath = path.Split(":");
                    path = splitPath[1];
                }

                path = path.Replace("\\", Path.DirectorySeparatorChar.ToString());
            }

            return path;
        }

        public static void PerformApplicationTest(List<string> argumentsUnderTest, string expectedMessage, List<RuleViolation> expectedRuleViolations, int expectedFileCount)
        {
            // arrange
            expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.Line).ToList();

            var appArgs = argumentsUnderTest.ToArray();
            var mockReporter = Substitute.For<IConsoleReporter>();

            var reportedViolations = new List<IRuleViolation>();
            mockReporter.When(reporter => reporter.ReportViolation(Arg.Any<IRuleViolation>())).Do(x => reportedViolations.Add(x.Arg<IRuleViolation>()));
            var reportedMessages = new List<string>();

            mockReporter.When(reporter => reporter.Report(Arg.Any<string>())).Do(x => reportedMessages.Add(x.Arg<string>()));
            var application = new Application(appArgs, mockReporter);

            // act
            application.Run();

            // assert
            reportedViolations = reportedViolations.OrderBy(o => o.Line).ThenBy(o => o.RuleName).ToList();
            expectedRuleViolations = expectedRuleViolations.OrderBy(o => o.Line).ThenBy(o => o.RuleName).ToList();
            Assert.AreEqual(expectedRuleViolations.Count, reportedViolations.Count);
            Assert.IsTrue((string.IsNullOrEmpty(expectedMessage) && reportedMessages.Count == 0) || string.Join(" ", reportedMessages).Contains(expectedMessage), $"Expected: '{expectedMessage}', Received: '{string.Join(" ", reportedMessages)}'");
            CollectionAssert.AreEqual(expectedRuleViolations, reportedViolations, RuleViolationComparer);
        }
    }
}
