using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Parser;

namespace TSQLLint.Tests.UnitTests.Parser
{
    [TestFixture]
    public class SqlStreamReaderBuilderTests
    {
        [Test]
        public void CreateReader_WithoutPlaceholders_DoesNotChangeSql()
        {
            // arrange
            var mockEnvironmentWrapper = Substitute.For<IEnvironmentWrapper>();
            var inputSql = "select 1;";

            // act
            var outputSql = new SqlStreamReaderBuilder(mockEnvironmentWrapper)
                .CreateReader(ParsingUtility.GenerateStreamFromString(inputSql))
                .ReadToEnd();

            // assert
            Assert.AreEqual(inputSql, outputSql);
        }

        [Test]
        public void CreateReader_WithPlaceholdersButNoEnvironmentVariables_DoesNotChangeSql()
        {
            // arrange
            var mockEnvironmentWrapper = Substitute.For<IEnvironmentWrapper>();
            mockEnvironmentWrapper.GetEnvironmentVariable("bar").ReturnsNull();
            var inputSql = "select 1 where foo = $(bar);";

            // act
            var outputSql = new SqlStreamReaderBuilder(mockEnvironmentWrapper)
                .CreateReader(ParsingUtility.GenerateStreamFromString(inputSql))
                .ReadToEnd();

            // assert
            Assert.AreEqual(inputSql, outputSql);
        }

        [Test]
        public void CreateReader_WithPlaceholdersAndAllEnvironmentVariables_ChangesSql()
        {
            // arrange
            var mockEnvironmentWrapper = Substitute.For<IEnvironmentWrapper>();
            mockEnvironmentWrapper.GetEnvironmentVariable("foo").Returns("1");
            mockEnvironmentWrapper.GetEnvironmentVariable("bar").Returns("bar");
            var inputSql = "select 1 where foo = $(foo) and bar = '$(bar)';";

            // act
            var outputSql = new SqlStreamReaderBuilder(mockEnvironmentWrapper)
                .CreateReader(ParsingUtility.GenerateStreamFromString(inputSql))
                .ReadToEnd();

            // assert
            Assert.AreEqual("select 1 where foo = 1 and bar = 'bar';", outputSql);
        }

        [Test]
        public void CreateReader_WithPlaceholdersAndSomeEnvironmentVariables_ChangesSql()
        {
            // arrange
            var mockEnvironmentWrapper = Substitute.For<IEnvironmentWrapper>();
            mockEnvironmentWrapper.GetEnvironmentVariable("foo").Returns("foo");
            mockEnvironmentWrapper.GetEnvironmentVariable("bar").ReturnsNull();
            var inputSql = "select 1 where foo = '$(foo)' and bar = '$(bar)';";

            // act
            var outputSql = new SqlStreamReaderBuilder(mockEnvironmentWrapper)
                .CreateReader(ParsingUtility.GenerateStreamFromString(inputSql))
                .ReadToEnd();

            // assert
            Assert.AreEqual("select 1 where foo = 'foo' and bar = '$(bar)';", outputSql);
        }
    }
}
