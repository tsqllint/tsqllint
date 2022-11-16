using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class DataCompressionOptionRule : BaseRuleVisitor, ISqlRule
    {
        public DataCompressionOptionRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "data-compression";

        public override string RULE_TEXT => "Expected table to use data compression";

        public override void Visit(CreateTableStatement node)
        {
            var childCompressionVisitor = new ChildCompressionVisitor();
            node.AcceptChildren(childCompressionVisitor);

            if (!childCompressionVisitor.CompressionOptionExists)
            {
                errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(node), GetColumnNumber(node));
            }
        }

        private class ChildCompressionVisitor : TSqlFragmentVisitor
        {
            public bool CompressionOptionExists
            {
                get;
                private set;
            }

            public override void Visit(DataCompressionOption node)
            {
                CompressionOptionExists = true;
            }
        }
    }
}
