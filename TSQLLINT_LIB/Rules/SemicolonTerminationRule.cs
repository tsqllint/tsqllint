using System;
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

        public SemicolonTerminationRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlStatement node)
        {
            var skipTypes = new[]
            {
                typeof(TryCatchStatement),
                typeof(WhileStatement),
                typeof(BeginEndBlockStatement),
                typeof(IfStatement),
                typeof(IndexDefinition)
            };

            if (skipTypes.Contains(node.GetType()))
            {
                return;
            }

            var lastToken = node.ScriptTokenStream[node.LastTokenIndex];
            var lastTokenType = lastToken.TokenType;
            if (lastTokenType != TSqlTokenType.Semicolon)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, lastToken.Line, lastToken.Column + lastToken.Text.Length);
            }
        }
    }
}
