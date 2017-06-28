using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interfaces;

namespace TSQLLINT_LIB.Rules
{
    public class TerminateStatementsWithSemicolonRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME {get { return "statement-semicolon-termination";}}
        public string RULE_TEXT { get { return "Terminate statements with semicolon"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public TerminateStatementsWithSemicolonRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlStatement node)
        {
            var lastToken = node.ScriptTokenStream[node.LastTokenIndex].TokenType;
            if (lastToken != TSqlTokenType.Semicolon)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
            }
        }
    }
}
