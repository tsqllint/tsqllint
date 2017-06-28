using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interfaces;

namespace TSQLLINT_LIB.Rules
{
    public class SchemaQualifyRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "schema-qualify"; } }
        public string RULE_TEXT { get { return "Schema qualify all object names"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public SchemaQualifyRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(SchemaObjectName node)
        {
            if (node.SchemaIdentifier == null)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
            }
        }
    }
}