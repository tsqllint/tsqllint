using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SemicolonRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME {get { return "statement-semicolon-termination";}}
        public string RULE_TEXT { get { return "Terminate statements with semicolon"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public SemicolonRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlStatement node)
        {
            if (node.GetType() == typeof(TryCatchStatement))
            {
                return;
            }

            var lastToken = node.ScriptTokenStream[node.LastTokenIndex].TokenType;
            if (lastToken != TSqlTokenType.Semicolon)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
            }
        }
    }
}
