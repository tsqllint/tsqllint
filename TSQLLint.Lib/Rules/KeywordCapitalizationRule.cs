using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Common;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class KeywordCapitalizationRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public KeywordCapitalizationRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "keyword-capitalization";

        public string RULE_TEXT => "Expected TSQL Keyword to be capitalized";

        public override void Visit(TSqlScript node)
        {
            for (var index = 0; index < node.ScriptTokenStream?.Count; index++)
            {
                var token = node.ScriptTokenStream[index];
                if (!Constants.TSqlKeywords.Contains(token.Text, StringComparer.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                if (IsUpperCase(token.Text))
                {
                    continue;
                }

                // get a count of all tabs on the line that occur prior to the last token in this node
                var tabsOnLine = ColumnNumberCalculator.CountTabsBeforeToken(token.Line, index, node.ScriptTokenStream);
                var column = ColumnNumberCalculator.GetColumnNumberBeforeToken(tabsOnLine, token);

                errorCallback(RULE_NAME, RULE_TEXT, token.Line, column);
            }
        }

        private static bool IsUpperCase(string input)
        {
            return input.All(t => !char.IsLetter(t) || char.IsUpper(t));
        }
    }
}
