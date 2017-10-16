using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLint.Lib.Rules.Common
{
    public static class ColumnNumberCounter
    {
        // count all tabs on a line up to the last token index
        public static int CountTabsOnLine(int lastTokenLine, int lastTokenIndex, IList<TSqlParserToken> tokens)
        {
            var tabCount = 0;
            for (var tokenIndex = 0; tokenIndex < lastTokenIndex; tokenIndex++)
            {
                var token = tokens[tokenIndex];
                if (token.Line != lastTokenLine || string.IsNullOrEmpty(token.Text))
                {
                    continue;
                }

                tabCount += token.Text.Count(t => t == '\t');
            }

            return tabCount;
        }

        public static int GetColumnNumberBeforeToken(int tabsOnLine, TSqlParserToken token)
        {
            return token.Column + (tabsOnLine * Constants.TabWidth - tabsOnLine);
        }

        public static int GetColumnNumberAfterToken(int tabsOnLine, TSqlParserToken token)
        {
            return token.Column + token.Text.Length + (tabsOnLine * Constants.TabWidth - tabsOnLine);
        }
    }
}
