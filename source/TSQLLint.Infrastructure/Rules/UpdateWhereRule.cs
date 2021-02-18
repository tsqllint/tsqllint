using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class UpdateWhereRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public UpdateWhereRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "update-where";

        public string RULE_TEXT => "Expected WHERE clause for UPDATE statement";

        public int DynamicSqlStartColumn { get; set; }

        public int DynamicSqlStartLine { get; set; }

        public override void Visit(UpdateStatement node)
        {
            Analyze(node);
        }

        private void Analyze(TSqlFragment node)
        {
            var whereVisitor = new WhereVisitor();
            node.AcceptChildren(whereVisitor);

            if (whereVisitor.WhereClauseFound)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, GetColumnNumber(node));
        }

        private class WhereVisitor : TSqlFragmentVisitor
        {
            public bool WhereClauseFound
            {
                get;
                private set;
            }

            public override void Visit(WhereClause node)
            {
                WhereClauseFound = true;
            }
        }

        private int GetColumnNumber(TSqlFragment node)
        {
            return node.StartLine == DynamicSqlStartLine
                ? node.StartColumn + DynamicSqlStartColumn
                : node.StartColumn;
        }
    }
}