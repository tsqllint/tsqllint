using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLint.Lib.Rules.Common
{
    public static class ColumnNumberCalculator
    {
        public static int GetNodeColumnPosition(TSqlFragment node)
        {
            var line = string.Empty;
            var firstLine = node.StartLine;
            var lastLine = node.ScriptTokenStream[node.LastTokenIndex].Line;

            for (var tokenIndex = 0; tokenIndex <= node.LastTokenIndex; tokenIndex++)
            {
                var token = node.ScriptTokenStream[tokenIndex];
                if (token.Line >= firstLine && token.Line <= lastLine)
                {
                    line += token.Text;
                }
            }

            var positionOfNodeOnLine = line.LastIndexOf(node.ScriptTokenStream[node.FirstTokenIndex].Text, StringComparison.Ordinal);
            var charactersBeforeNode = line.Substring(0, positionOfNodeOnLine);

            var offSet = 0;
            if (charactersBeforeNode.IndexOf(" ", StringComparison.Ordinal) != -1)
            {
                offSet = 1;
            }
            
            var tabCount = charactersBeforeNode.Count(t => t == '\t');
            var totalTabLentgh = tabCount * Constants.TabWidth;
            
            var nodePosition = totalTabLentgh + (charactersBeforeNode.Length - tabCount) + offSet;
            return nodePosition;
        }

        // count all tabs on a line up to the last token index
        public static int CountTabsBeforeToken(int lastTokenLine, int lastTokenIndex, IList<TSqlParserToken> tokens)
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
