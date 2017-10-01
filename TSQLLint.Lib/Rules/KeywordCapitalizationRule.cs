using System;
using System.Linq;
using TSQLLint.Lib.Rules.Common;
using TSQLLint.Lib.Rules.Interface;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLint.Lib.Rules
{
    public class KeywordCapitalizationRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME
        {
            get
            {
                return "keyword-capitalization";
            }
        }

        public string RULE_TEXT
        {
            get
            {
                return "Expected TSQL Keyword to be capitalized";
            }
        }

        private readonly Action<string, string, int, int> ErrorCallback;

        public KeywordCapitalizationRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlScript node)
        {
            for (var index = 0; index < node.ScriptTokenStream.Count; index++)
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
                var tabsOnLine = ColumnNumberCounter.CountTabsOnLine(token.Line, index, node.ScriptTokenStream);
                var column = ColumnNumberCounter.GetColumnNumberBeforeToken(tabsOnLine, token);

                ErrorCallback(RULE_NAME, RULE_TEXT, token.Line, column);
            }
        }

        private static bool IsUpperCase(string input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                if (char.IsLetter(input[i]) && !char.IsUpper(input[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}