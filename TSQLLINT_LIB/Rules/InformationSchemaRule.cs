using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interfaces;

namespace TSQLLINT_LIB.Rules
{
    public class InfirmationSchemaRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "information-schema"; } }
        public string RULE_TEXT { get { return "Do not use the INFORMATION_SCHEMA views"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public InfirmationSchemaRule(Action<string, string, TSqlFragment> errorCallback)
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