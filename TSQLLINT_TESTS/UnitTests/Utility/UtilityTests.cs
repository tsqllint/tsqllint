using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TSQLLINT_LIB;

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
        Assert.IsFalse(Utility.tryParseJson(testString, out token));
        Assert.IsNull(token);
    }

    [TestCase("{}")]
    [TestCase("{ \"foo\": \"bar\"}")]
    [TestCase("99")]
    public void ValidJson(string testString)
    {
        JToken token;
        Assert.IsTrue(Utility.tryParseJson(testString, out token));
        Assert.IsNotNull(token);
    }
}