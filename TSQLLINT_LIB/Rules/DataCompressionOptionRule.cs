using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;

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
            var compressionOptionExists = false;
            foreach (var tableOption in node.Options)
            {
                if (tableOption.OptionKind == TableOptionKind.DataCompression)
                {
                    compressionOptionExists = true;
                }
            }

            if (!compressionOptionExists)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
            }
        }
    }
}