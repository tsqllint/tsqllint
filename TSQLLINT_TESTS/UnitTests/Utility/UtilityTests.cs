using NUnit.Framework;
using TSQLLINT_LIB;

public class UtilityTests
{
    [TestCase("{")]
    [TestCase("}")]
    [TestCase("")]
    public void InvalidJson(string testString)
    {
        Assert.IsFalse(Utility.IsValidJson(testString));
    }

    [TestCase("{}")]
    [TestCase("{ \"foo\": \"bar\"}")]
    public void ValidJson(string testString)
    {
        Assert.IsTrue(Utility.IsValidJson(testString));
    }
}