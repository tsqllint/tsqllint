using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class InformationSchemaRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public InformationSchemaRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "information-schema";

        public string RULE_TEXT => "Expected use of SYS.Partitions rather than INFORMATION_SCHEMA views";

        public override void Visit(SchemaObjectName node)
        {
            var schemaIdentifier = node.SchemaIdentifier?.Value != null;

            if (schemaIdentifier && node.SchemaIdentifier.Value.Equals("INFORMATION_SCHEMA", StringComparison.InvariantCultureIgnoreCase))
            {
                errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            }
        }
    }
}
