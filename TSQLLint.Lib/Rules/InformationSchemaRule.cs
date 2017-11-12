using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Standard.Rules.Interface;

namespace TSQLLint.Lib.Standard.Rules
{
    public class InformationSchemaRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "information-schema";

        public string RULE_TEXT => "Expected use of SYS.Partitions rather than INFORMATION_SCHEMA views";

        private readonly Action<string, string, int, int> ErrorCallback;

        public InformationSchemaRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(SchemaObjectName node)
        {
            var schemaIdentifier = node.SchemaIdentifier?.Value != null;

            if (schemaIdentifier && node.SchemaIdentifier.Value.Equals("INFORMATION_SCHEMA", StringComparison.InvariantCultureIgnoreCase))
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            }
        }
    }
}
