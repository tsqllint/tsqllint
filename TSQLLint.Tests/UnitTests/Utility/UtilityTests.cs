using NUnit.Framework;

namespace TSQLLint.Tests.UnitTests.Utility
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
            Assert.IsFalse(Lib.Utility.ParsingUtility.TryParseJson(testString, out var token));
            Assert.IsNull(token);
        }

        [TestCase("{}")]
        [TestCase("{ \"foo\": \"ActivatePlugins_ReportViolations_ShouldCallReporter\"}")]
        [TestCase("99")]
        public void ValidJson(string testString)
        {
            Assert.IsTrue(Lib.Utility.ParsingUtility.TryParseJson(testString, out var token));
            Assert.IsNotNull(token);
        }
    }
}
