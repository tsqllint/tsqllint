using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class CrossDatabaseRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME
        {
            get
            {
                return "cross-database";
            }
        }

        public string RULE_TEXT
        {
            get
            {
                return "Cross database queries can cause performance problems and are discouraged";
            }
        }

        private readonly Action<string, string, int, int> ErrorCallback;

        public CrossDatabaseRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(NamedTableReference node)
        {
            if (node.SchemaObject.DatabaseIdentifier == null || node.SchemaObject.ServerIdentifier != null)
            {
                return;
            }

            ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
        }
    }
}
