using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SchemaQualifyRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "schema-qualify"; } }
        public string RULE_TEXT { get { return "Schema qualify all object names"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        private List<string> TableAliases = new List<string>
        {
            "INSERTED",
            "UPDATED",
            "DELETED"
        };

        public SchemaQualifyRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlStatement node)
        {
            var childAliasVisitor = new ChildAliasVisitor();
            node.AcceptChildren(childAliasVisitor);
            TableAliases.AddRange(childAliasVisitor.TableAliases);
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
            if (TableAliases.FindIndex(x => x.Equals(node.SchemaObject.BaseIdentifier.Value, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                return;
            }

            ErrorCallback(RULE_NAME, RULE_TEXT, node);
        }

        public class ChildAliasVisitor : TSqlFragmentVisitor
        {
            public List<string> TableAliases = new List<string>();

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