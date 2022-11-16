using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class SchemaQualifyRule : BaseRuleVisitor, ISqlRule
    {
        private readonly List<string> tableAliases = new ()
        {
            "INSERTED",
            "UPDATED",
            "DELETED"
        };

        public SchemaQualifyRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "schema-qualify";

        public override string RULE_TEXT => "Object name not schema qualified";

        public override void Visit(TSqlStatement node)
        {
            var childAliasVisitor = new ChildAliasVisitor();
            node.AcceptChildren(childAliasVisitor);
            tableAliases.AddRange(childAliasVisitor.TableAliases);
        }

        public override void Visit(NamedTableReference node)
        {
            VisitTableName(node.SchemaObject, true);
        }

        public override void Visit(CreateTableStatement node)
        {
            VisitTableName(node.SchemaObjectName, false);
        }

        public override void Visit(AlterTableStatement node)
        {
            VisitTableName(node.SchemaObjectName, false);
        }

        public override void Visit(TruncateTableStatement node)
        {
            VisitTableName(node.TableName, false);
        }

        public override void Visit(DropTableStatement node)
        {
            foreach (var schemaObjectName in node.Objects)
            {
                VisitTableName(schemaObjectName, false);
            }
        }

        private void VisitTableName(SchemaObjectName node, bool canHaveTableAliases)
        {
            if (node.SchemaIdentifier != null)
            {
                return;
            }

            // don't attempt to enforce schema validation on temp tables
            if (node.BaseIdentifier.Value.Contains('#'))
            {
                return;
            }

            // don't attempt to enforce schema validation on table aliases
            if (canHaveTableAliases && tableAliases.Exists(x => x.Equals(node.BaseIdentifier.Value, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(node), GetColumnNumber(node));
        }

        public class ChildAliasVisitor : TSqlFragmentVisitor
        {
            public List<string> TableAliases { get; } = new ();

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
