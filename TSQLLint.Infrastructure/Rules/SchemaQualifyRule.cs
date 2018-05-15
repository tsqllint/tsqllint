using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class SchemaQualifyRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        private readonly List<string> tableAliases = new List<string>
        {
            "INSERTED",
            "UPDATED",
            "DELETED"
        };

        public SchemaQualifyRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "schema-qualify";

        public string RULE_TEXT => "Object name not schema qualified";

        public int DynamicSqlStartColumn { get; set; }

        public override void Visit(TSqlStatement node)
        {
            var childAliasVisitor = new ChildAliasVisitor();
            node.AcceptChildren(childAliasVisitor);
            tableAliases.AddRange(childAliasVisitor.TableAliases);
        }

        public override void Visit(NamedTableReference node)
        {
            if (node.SchemaObject.SchemaIdentifier != null)
            {
                return;
            }

            // don't attempt to enforce schema validation on temp tables
            if (node.SchemaObject.BaseIdentifier.Value.Contains("#"))
            {
                return;
            }

            // don't attempt to enforce schema validation on table aliases
            if (tableAliases.FindIndex(x => x.Equals(node.SchemaObject.BaseIdentifier.Value, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
        }

        public class ChildAliasVisitor : TSqlFragmentVisitor
        {
            public List<string> TableAliases { get; } = new List<string>();

            public override void Visit(TableReferenceWithAlias node)
            {
                if (node.Alias != null)
                {
                    TableAliases.Add(node.Alias.Value);
                }
            }

            public override void Visit(CommonTableExpression node)
            {
                TableAliases.Add(node.ExpressionName.Value);
            }
        }
    }
}
