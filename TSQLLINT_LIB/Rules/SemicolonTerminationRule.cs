using System;
using System.CodeDom;
using System.Linq.Expressions;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SemicolonTerminationRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME {get { return "semicolon-termination";}}
        public string RULE_TEXT { get { return "Terminate statements with semicolon"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public SemicolonTerminationRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlStatement node)
        {
            if (typeof(TryCatchStatement) == node.GetType() 
                || typeof(WhileStatement) == node.GetType()
                || typeof(BeginEndBlockStatement) == node.GetType())
            {
                return;
            }

            var lastToken = node.ScriptTokenStream[node.LastTokenIndex];
            var lastTokenType = lastToken.TokenType;
            if (lastTokenType != TSqlTokenType.Semicolon)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
            }
        }
    }
}
