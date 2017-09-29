using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace TSQLLINT_LIB_TESTS.UnitTests.Utility
{
    public class UtilityTests
    {
        [TestCase("{")]
        [TestCase("}")]
        [TestCase("")]
        [TestCase("{{}")]
        [TestCase("Foo")]
        public void InvalidJson(string testString)
        {
            JToken token;
            Assert.IsFalse(TSQLLINT_LIB.Utility.Utility.TryParseJson(testString, out token));
            Assert.IsNull(token);
        }

        [TestCase("{}")]
        [TestCase("{ \"foo\": \"ActivatePlugins_ReportViolations_ShouldCallReporter\"}")]
        [TestCase("99")]
        public void ValidJson(string testString)
        {
            JToken token;
            Assert.IsTrue(TSQLLINT_LIB.Utility.Utility.TryParseJson(testString, out token));
            Assert.IsNotNull(token);
        }
    }
}