using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using TSQLLint.Infrastructure.Parser;

namespace TSQLLint.Tests.UnitTests.Parser
{
    [TestFixture]
    public class DynamicSQLParser_Tests
    {
        private const bool ShouldCallBack = true;
        private const bool ShouldNotCallback = false;

        private static readonly object[] TestCases =
        {
            new object[]
            {
                "Execute string",
                "EXEC('SELECT FOO FROM BAR')",
                "SELECT FOO FROM BAR",
                ShouldCallBack
            },
            new object[]
            {
                "Execute binary expression containing string literals",
                "EXEC('SELECT FOO' + ' FROM BAR')",
                "SELECT FOO FROM BAR",
                ShouldCallBack
            },
            new object[]
            {
                "Execute string consisting of scalar command, should not callback",
                @"DECLARE @sqlCommand int
                    SET @sqlCommand = 1 - 1
                    EXEC ('SELECT ' +  @sqlCommand)",
                string.Empty,
                ShouldNotCallback
            },
            new object[]
            {
                "Execute var",
                @"DECLARE @sqlCommand varchar(1000)
                    SET @sqlCommand = 'SELECT FOO FROM BAR'
                    EXEC (@sqlCommand)",
                "SELECT FOO FROM BAR",
                ShouldCallBack
            },
            new object[]
            {
                "Execute var consisting of binary expression",
                @"DECLARE @sqlCommand varchar(1000)
                    SET @sqlCommand = 'SELECT FOO' + ' FROM BAR'
                    EXEC (@sqlCommand)",
                "SELECT FOO FROM BAR",
                ShouldCallBack
            },
            new object[]
            {
                "Execute var, dynamic object creation",
                @"DECLARE @Sql nvarchar(4000);
                    SET @Sql = 'CREATE PROCEDURE dbo.Foo AS RETURN 0';
                    EXEC (@Sql);",
                "CREATE PROCEDURE dbo.Foo AS RETURN 0",
                ShouldCallBack
            },
            new object[]
            {
              "Multi-variable Concatenation",
              @"DECLARE @sqlCommandOne varchar(1000)
                  DECLARE @sqlCommandTwo varchar(1000)
                  SET @sqlCommandOne = 'SELECT FOO'
                  SET @sqlCommandTwo = ' FROM BAR'
                  SET @sqlCommandThree = ' WHERE BAR IS NULL'
                  EXEC (@sqlCommandOne + @sqlCommandTwo + @sqlCommandThree)",
              "SELECT FOO FROM BAR WHERE BAR IS NULL",
              ShouldCallBack
            },
            new object[]
            {
                "Execute var mixed expression types",
                @"DECLARE @sqlCommandOne varchar(1000)
                      DECLARE @sqlCommandTwo int
                      SET @sqlCommandOne = 'SELECT '
                      SET @sqlCommandTwo = 1
                      EXEC (@sqlCommandOne + @sqlCommandTwo)",
                "SELECT 1",
                ShouldCallBack
            },
            new object[]
            {
            "Variable String Concatenation",
            @"DECLARE @sqlCommandOne varchar(1000)
                  DECLARE @sqlCommandTwo varchar(1000)
                  SET @sqlCommandOne = 'SELECT FOO '
                  EXEC (@sqlCommandOne + 'FROM BAR')",
            "SELECT FOO FROM BAR",
            ShouldCallBack
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void ShouldParse(string description, string executeStatement, string innerSql, bool expectCallback = true)
        {
            var stream = ParsingUtility.GenerateStreamFromString(executeStatement);

            var receivedCallback = false;
            void DynamicCallback(string dynamicSQL)
            {
                Assert.AreEqual(innerSql, dynamicSQL);
                receivedCallback = true;
            }

            var visitor = new DynamicSQLParser(DynamicCallback);
            var fragmentBuilder = new FragmentBuilder();
            var textReader = new StreamReader(stream);

            var sqlFragment = fragmentBuilder.GetFragment(textReader, out var errors);
            CollectionAssert.IsEmpty(errors, "parsing errors were generated");

            sqlFragment.Accept(visitor);

            Assert.AreEqual(receivedCallback, expectCallback);
        }

        [ExcludeFromCodeCoverage]
        [TestCase("SELECT 1")]
        [TestCase("EXEC(@Foo)")]
        public void ShouldIgnore(string testString)
        {
            var stream = ParsingUtility.GenerateStreamFromString(testString);

            void DynamicCallback(string dynamicSQL)
            {
                Assert.Fail();
            }

            var visitor = new DynamicSQLParser(DynamicCallback);
            var fragmentBuilder = new FragmentBuilder();
            var textReader = new StreamReader(stream);
            var sqlFragment = fragmentBuilder.GetFragment(textReader, out var errors);
            sqlFragment.Accept(visitor);

            CollectionAssert.IsEmpty(errors, "parsing errors were generated");
        }
    }
}
