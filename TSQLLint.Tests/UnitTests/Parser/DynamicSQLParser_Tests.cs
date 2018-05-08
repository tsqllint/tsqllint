using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using TSQLLint.Infrastructure.Parser;

namespace TSQLLint.Tests.UnitTests.Parser
{
    [TestFixture]
    public class DynamicSQLParser_Tests
    {
        private static readonly object[] TestCases =
        {
            new object[]
            {
                "Execute string",
                "EXEC('SELECT FOO FROM BAR')",
                "SELECT FOO FROM BAR"
            },
            new object[]
            {
                "Execute binary expression containing string literals",
                "EXEC('SELECT FOO' + ' FROM BAR')",
                "SELECT FOO FROM BAR"
            },
            new object[]
            {
                "Execute var",
                @"DECLARE @sqlCommand varchar(1000)
                    SET @sqlCommand = 'SELECT FOO FROM BAR'
                    EXEC (@sqlCommand)",
                "SELECT FOO FROM BAR"
            },
            new object[]
            {
                "Execute var consisting of binary expression",
                @"DECLARE @sqlCommand varchar(1000)
                    SET @sqlCommand = 'SELECT FOO' + ' FROM BAR'
                    EXEC (@sqlCommand)",
                "SELECT FOO FROM BAR"
            },
            new object[]
            {
                "Execute var, dynamic object creation",
                @"DECLARE @Sql nvarchar(4000);
                    SET @Sql = 'CREATE PROCEDURE dbo.Foo AS RETURN 0';
                    EXEC (@Sql);",
                "CREATE PROCEDURE dbo.Foo AS RETURN 0"
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
              "SELECT FOO FROM BAR WHERE BAR IS NULL"
            },
            new object[]
            {
            "Variable String Concatenation",
            @"DECLARE @sqlCommandOne varchar(1000)
                  DECLARE @sqlCommandTwo varchar(1000)
                  SET @sqlCommandOne = 'SELECT FOO '
                  EXEC (@sqlCommandOne + 'FROM BAR')",
            "SELECT FOO FROM BAR"
            }
        };

        [TestCaseSource(nameof(TestCases))]
        public void ShouldParse(string description, string executeStatement, string innerSql)
        {
            var stream = ParsingUtility.GenerateStreamFromString(executeStatement);

            var callbackExecuted = false;
            void DynamicCallback(string dynamicSQL)
            {
                Assert.AreEqual(innerSql, dynamicSQL);
                callbackExecuted = true;
            }

            var visitor = new DynamicSQLParser(DynamicCallback);
            var fragmentBuilder = new FragmentBuilder();
            var textReader = new StreamReader(stream);
            var sqlFragment = fragmentBuilder.GetFragment(textReader, out var errors);
            sqlFragment.Accept(visitor);

            Assert.IsTrue(callbackExecuted, "callback not executed");
            CollectionAssert.IsEmpty(errors, "parsing errors were generated");
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
