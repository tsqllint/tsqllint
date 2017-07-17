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

        public override void Visit(TryCatchStatement node)
        {
            var childBeginEndBlockVisitor = new ChildBeginEndBlockVisitor();
            node.AcceptChildren(childBeginEndBlockVisitor);

            if (childBeginEndBlockVisitor.ErrorLoged)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, childBeginEndBlockVisitor.ErrorNode);
            }
        }

        public override void Visit(TSqlStatement node)
        {
            var childBeginEndBlockVisitor = new ChildBeginEndBlockVisitor();
            node.AcceptChildren(childBeginEndBlockVisitor);

            if (childBeginEndBlockVisitor.ErrorLoged)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, childBeginEndBlockVisitor.ErrorNode);
                return;
            }

            var lastToken = node.ScriptTokenStream[node.LastTokenIndex].TokenType;
            if (lastToken != TSqlTokenType.Semicolon)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
            }
        }

        public class ChildBeginEndBlockVisitor : TSqlFragmentVisitor
        {
            public bool ErrorLoged;
            public BeginEndBlockStatement ErrorNode;

            public override void Visit(BeginEndBlockStatement node)
            {
                var lastToken = node.ScriptTokenStream[node.LastTokenIndex].TokenType;
                if (lastToken != TSqlTokenType.Semicolon)
                {
                    ErrorLoged = true;
                    ErrorNode = node;
                }
            }
        }
    }
}
