using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLINT_LIB.Rules
{
    public class InformationSchemaRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "information-schema"; } }
        public string RULE_TEXT { get { return "Do not use the INFORMATION_SCHEMA views"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public InformationSchemaRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(SchemaObjectName node)
        {
            var databaseIdentifier = node.DatabaseIdentifier != null && node.DatabaseIdentifier.Value != null;
            var schemaIdentifier = node.SchemaIdentifier != null && node.SchemaIdentifier.Value != null;

            if (databaseIdentifier 
                && schemaIdentifier 
                && node.SchemaIdentifier.Value.Equals("INFORMATION_SCHEMA", StringComparison.InvariantCultureIgnoreCase))
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
            }
        }
    }
}