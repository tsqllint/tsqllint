using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Common;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class SemicolonTerminationRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "semicolon-termination";

        public string RULE_TEXT => "Statement not terminated with semicolon";

        private readonly Action<string, string, int, int> ErrorCallback;

        // don't enforce semicolon termination on these statements
        private readonly Type[] TypesToSkip = 
        {
            typeof(BeginEndBlockStatement),
            typeof(GoToStatement),
            typeof(IndexDefinition),
            typeof(LabelStatement),
            typeof(TryCatchStatement),
            typeof(WhileStatement),
            typeof(IfStatement)
        };

        public SemicolonTerminationRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlStatement node)
        {
            if (TypesToSkip.Contains(node.GetType()) || EndsWithSemicolon(node))
            {
                return;
            }

            var lastToken = node.ScriptTokenStream[node.LastTokenIndex];
            var tabsOnLine = ColumnNumberCalculator.CountTabsBeforeToken(lastToken.Line, node.LastTokenIndex, node.ScriptTokenStream);
            var column = ColumnNumberCalculator.GetColumnNumberAfterToken(tabsOnLine, lastToken);
            ErrorCallback(RULE_NAME, RULE_TEXT, lastToken.Line, column);
        }

        public override void Visit(BeginEndBlockStatement node)
        {
            if (EndsWithSemicolon(node))
            {
                return;
            }

            var endTerminator = node.ScriptTokenStream[node.LastTokenIndex];
            ErrorCallback(
                RULE_NAME,
                RULE_TEXT,
                node.ScriptTokenStream[node.LastTokenIndex].Line,
                endTerminator.Column + endTerminator.Text.Length);
        }

        private static bool EndsWithSemicolon(TSqlFragment node)
        {
            return node.ScriptTokenStream[node.LastTokenIndex].TokenType == TSqlTokenType.Semicolon
                || node.ScriptTokenStream[node.LastTokenIndex + 1].TokenType == TSqlTokenType.Semicolon;
        }
    }
}
