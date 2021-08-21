using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Text;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class PositionalInsertRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public PositionalInsertRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "positional-insert";

        public string RULE_TEXT => "INSERT INTO table VALUES disallowed. Suggest INSERT INTO table(<columns>) VALUES";

        public int DynamicSqlStartColumn { get; set; }

        public int DynamicSqlStartLine { get; set; }
        public override void Visit(InsertSpecification node)
        {
            var columns = node.Columns;
            if (columns == null || columns.Count <= 0)
            {
                errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, GetColumnNumber(node));
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
