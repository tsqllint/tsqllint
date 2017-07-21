using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SemicolonTerminationRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME {get { return "semicolon-termination";}}
        public string RULE_TEXT { get { return "Terminate statements with semicolon"; } }
        public Action<string, string, int, int> ErrorCallback;

        private int TabLength = 4;

        // don't enforce semicolon termination on these statements
        private readonly Type[] TypesToSkip = {
            typeof(BeginEndBlockStatement),
            typeof(IfStatement),
            typeof(IndexDefinition),
            typeof(TryCatchStatement),
            typeof(WhileStatement)
        };

        public SemicolonTerminationRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlStatement node)
        {
            if (TypesToSkip.Contains(node.GetType()))
            {
                return;
            }

            var lastToken = node.ScriptTokenStream[node.LastTokenIndex];
            if (lastToken.TokenType == TSqlTokenType.Semicolon)
            {
                return;
            }

            // get a count of all tabs on the line that occur prior to the last token in this node
            var tabsOnLine = CountTabsOnLine(lastToken.Line, node.LastTokenIndex, node.ScriptTokenStream);
            var column = lastToken.Column + lastToken.Text.Length + ((tabsOnLine * TabLength) - tabsOnLine);
            ErrorCallback(RULE_NAME, RULE_TEXT, lastToken.Line, column);
        }

        public override void Visit(BeginEndBlockStatement node)
        {
            var beginTerminator = node.ScriptTokenStream[node.FirstTokenIndex + 1];
            if (beginTerminator.TokenType != TSqlTokenType.Semicolon)
            {
                ErrorCallback(RULE_NAME, 
                    RULE_TEXT, 
                    node.ScriptTokenStream[node.FirstTokenIndex].Line, 
                    node.StartColumn + beginTerminator.Column);
            }

            var endTerminator = node.ScriptTokenStream[node.LastTokenIndex];
            if (endTerminator.TokenType != TSqlTokenType.Semicolon)
            {
                ErrorCallback(RULE_NAME, 
                    RULE_TEXT, 
                    node.ScriptTokenStream[node.LastTokenIndex].Line, 
                    endTerminator.Column + endTerminator.Text.Length);
            }
        }

        // count all tabs on a line up to the last token index
        private int CountTabsOnLine(int lastTokenLine, int lastTokenIndex, IList<TSqlParserToken> tokens)
        {
            var tabCount = 0;
            for (var tokenIndex = 0; tokenIndex < lastTokenIndex; tokenIndex++)
            {
                var token = tokens[tokenIndex];
                if (token.Line != lastTokenLine || string.IsNullOrEmpty(token.Text))
                {
                    continue;
                }

                for (var charIndex = 0; charIndex < token.Text.Length; charIndex++)
                {
                    if (token.Text[charIndex] == '\t')
                    {
                        tabCount++;
                    }
                }
            }
            return tabCount;
        }
    }
}