using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using NUnit.Framework;
using TSQLLint.Core;
using TSQLLint.Infrastructure.Configuration.Overrides;
using TSQLLint.Infrastructure.Parser;

namespace TSQLLint.Tests.UnitTests.Parser
{
    [TestFixture]
    public class InlineOverride_Tests
    {
        private static readonly object[] TestCases =
        {
            // Test misspelled "compatability-level"
            new object[]
            {
                "Compatibility Level Override Old",
                @"/* tsqllint-override compatability-level = 130 */
                  SELECT * FROM FOO;",
                1
            },
            new object[]
            {
                "Compatibility Level Override",
                @"/* tsqllint-override compatibility-level = 130 */
                  SELECT * FROM FOO;",
                1
            },
            new object[]
            {
                "No Overrides",
                @"SELECT * FROM FOO;",
                0
            },
            new object[]
            {
                "Malformed Override",
                @"/* tsqllint-overrides compatibility-level = 130 */
                  SELECT * FROM FOO;",
                0
            }
        };

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void Test(string description, string sql, int expectedOverrideCount)
        {
            var fragmentBuilder = new FragmentBuilder(Constants.DefaultCompatabilityLevel);
            var overrideFider = new OverrideFinder();

            var sqlStream = ParsingUtility.GenerateStreamFromString(sql);
            var overrides = overrideFider.GetOverrideList(sqlStream);

            var fragment = fragmentBuilder.GetFragment(new StreamReader(sqlStream), out var errors, overrides);

            Assert.AreEqual(expectedOverrideCount, overrides.Count());
            Assert.IsEmpty(errors);
            Assert.IsNotNull(fragment);
        }
    }
}
