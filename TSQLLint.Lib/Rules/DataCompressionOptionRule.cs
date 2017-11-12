using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Standard.Rules.Interface;

namespace TSQLLint.Lib.Standard.Rules
{
    public class DataCompressionOptionRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "data-compression";

        public string RULE_TEXT => "Expected table to use data compression";

        private readonly Action<string, string, int, int> ErrorCallback;

        public DataCompressionOptionRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(CreateTableStatement node)
        {
            var childCompressionVisitor = new ChildCompressionVisitor();
            node.AcceptChildren(childCompressionVisitor);

            if (!childCompressionVisitor.CompressionOptionExists)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            }
        }

        public class ChildCompressionVisitor : TSqlFragmentVisitor
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
