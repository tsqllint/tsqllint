using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class DataCompressionOptionRule : TSqlFragmentVisitor, ISqlRule
    {
        public DataCompressionOptionRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "data-compression"; }
        }

        public string RuleText
        {
            get { return "Expected table to use data compression"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(CreateTableStatement node)
        {
            var childCompressionVisitor = new ChildCompressionVisitor();
            node.AcceptChildren(childCompressionVisitor);

            if (!childCompressionVisitor.CompressionOptionExists)
            {
                ErrorCallback(RuleName, RuleText, node.StartLine, node.StartColumn);
            }
        }

        public class ChildCompressionVisitor : TSqlFragmentVisitor
        {
            public bool CompressionOptionExists { get; set; }

            public override void Visit(DataCompressionOption node)
            {
                this.CompressionOptionExists = true;
            }
        }
    }
}