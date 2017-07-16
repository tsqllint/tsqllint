using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class InformationSchemaRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "information-schema"; } }
        public string RULE_TEXT { get { return "Do not use the INFORMATION_SCHEMA views, use SYS.Partitions."; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public InformationSchemaRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(SchemaObjectName node)
        {
            var schemaIdentifier = node.SchemaIdentifier != null && node.SchemaIdentifier.Value != null;

            if (schemaIdentifier && node.SchemaIdentifier.Value.Equals("INFORMATION_SCHEMA", StringComparison.InvariantCultureIgnoreCase))
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
            }
        }
    }
}