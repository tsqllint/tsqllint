using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using TSQLLint.Core;

namespace TSQLLint.Infrastructure.Rules.Common
{
    public static class ColumnNumberCalculator
    {
        public static int GetNodeColumnPosition(TSqlFragment node)
        {
            var line = string.Empty;
            var nodeStartLine = node.StartLine;
            var nodeLastLine = node.ScriptTokenStream[node.LastTokenIndex].Line;

            for (var tokenIndex = 0; tokenIndex <= node.LastTokenIndex; tokenIndex++)
            {
                var token = node.ScriptTokenStream[tokenIndex];
                if (token.Line >= nodeStartLine && token.Line <= nodeLastLine)
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

            var tabCount = CountTabs(charactersBeforeNode);
            var totalTabLength = tabCount * Constants.TabWidth;

            var nodePosition = totalTabLength + (charactersBeforeNode.Length - tabCount) + offSet;
            return nodePosition;
        }

        /// <summary>
        /// Returns -1 if can't be found
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static int GetIndex(string line, int column)
        {
            var index = 0;
            while (index != column)
            {
                if (line.Length <= index)
                {
                    return -1;
                }

                if (line[index] == '\t')
                {
                    column -= (Constants.TabWidth - 1);
                }
                index++;
            }
            return column - 1;
        }

        private static int CountTabs(string charactersBeforeNode)
        {
            int tabCount = 0;
            foreach (var c in charactersBeforeNode)
            {
                if (c == '\t')
                {
                    tabCount++;
                }
            }

            return tabCount;
        }

        // count all tabs on a line up to the last token index
        public static int CountTabsBeforeToken(int lastTokenLine, int lastTokenIndex, IEnumerable<TSqlParserToken> tokens)
        {
            var tabCount = 0;
            var sqlParserTokens = tokens as TSqlParserToken[] ?? tokens.ToArray();

            for (var tokenIndex = 0; tokenIndex < lastTokenIndex; tokenIndex++)
            {
                var token = sqlParserTokens[tokenIndex];
                if (token.Line != lastTokenLine || string.IsNullOrEmpty(token.Text))
                {
                    continue;
                }

                tabCount += CountTabs(token.Text);
            }

            return tabCount;
        }

        public static int GetColumnNumberBeforeToken(int tabsOnLine, TSqlParserToken token)
        {
            return token.Column + ((tabsOnLine * Constants.TabWidth) - tabsOnLine);
        }

        public static int GetColumnNumberAfterToken(int tabsOnLine, TSqlParserToken token)
        {
            return token.Column + token.Text.Length + ((tabsOnLine * Constants.TabWidth) - tabsOnLine);
        }
    }
}
