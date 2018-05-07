using NUnit.Framework;
using TSQLLint.Infrastructure.Parser;

namespace TSQLLint.Tests.UnitTests.UtilityTests
{
    public class UtilityTests
    {
        [TestCase("{")]
        [TestCase("}")]
        [TestCase("")]
        [TestCase("{{}")]
        [TestCase("ShouldLogWhenLoggingEnables")]
        public void InvalidJson(string testString)
        {
            Assert.IsFalse(ParsingUtility.TryParseJson(testString, out var token));
            Assert.IsNull(token);
        }

        [TestCase("{}")]
        [TestCase("{ \"foo\": \"ActivatePlugins_ReportViolations_ShouldCallReporter\"}")]
        [TestCase("99")]
        public void ValidJson(string testString)
        {
            Assert.IsTrue(ParsingUtility.TryParseJson(testString, out var token));
            Assert.IsNotNull(token);
        }
    }
}
