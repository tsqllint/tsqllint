using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class LinkedServerRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME
        {
            get
            {
                return "linked-server";
            }
        }

        public string RULE_TEXT
        {
            get
            {
                return "Linked server calls can cause table locking and are discouraged";
            }
        }

        private readonly Action<string, string, int, int> ErrorCallback;

        public LinkedServerRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(NamedTableReference node)
        {
            if (node.SchemaObject.ServerIdentifier == null)
            {
                return;
            }

            ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
        }
    }
}
