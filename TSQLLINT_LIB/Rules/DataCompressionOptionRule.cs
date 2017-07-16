using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class DataCompressionOptionRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "data-compression"; } }
        public string RULE_TEXT { get { return "All Table and indexes including, Temp tables, should be compressed appropriately"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public DataCompressionOptionRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(CreateTableStatement node)
        {
            var childCompressionVisitor = new childCompressionVisitor();
            node.AcceptChildren(childCompressionVisitor);

            if (!childCompressionVisitor.compressionOptionExists)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
            }
        }

        public class childCompressionVisitor : TSqlFragmentVisitor
        {
            public bool compressionOptionExists;

            public override void Visit(DataCompressionOption node)
            {
                compressionOptionExists = true;
            }
        }
    }
}