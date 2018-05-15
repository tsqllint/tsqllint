using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class LinkedServerRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public LinkedServerRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "linked-server";

        public string RULE_TEXT => "Linked server queries can cause table locking and are discouraged";

        public int DynamicSqlOffset { get; set; }

        public override void Visit(NamedTableReference node)
        {
            if (node.SchemaObject.ServerIdentifier == null)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
        }
    }
}
