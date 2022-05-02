using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class NamedContraintRule : BaseRuleVisitor, ISqlRule
    {
        public NamedContraintRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "named-constraint";

        public override string RULE_TEXT => "Named constraints in temp tables can cause collisions when run in parallel";

        public override void Visit(CreateTableStatement node)
        {
            // only apply rule to temp tables
            if (!node.SchemaObjectName.BaseIdentifier.Value.Contains("#"))
            {
                return;
            }

            var constraintVisitor = new ConstraintVisitor();
            node.AcceptChildren(constraintVisitor);

            if (constraintVisitor.NamedConstraintExists)
            {
                errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, GetColumnNumber(node));
            }
        }

        private class ConstraintVisitor : TSqlFragmentVisitor
        {
            public bool NamedConstraintExists
            {
                get;
                private set;
            }

            public override void Visit(ConstraintDefinition node)
            {
                if (NamedConstraintExists)
                {
                    return;
                }

                NamedConstraintExists = node.ConstraintIdentifier != null;
            }
        }

        private int GetColumnNumber(TSqlFragment node)
        {
            return node.StartLine == DynamicSqlStartLine
                ? node.StartColumn + DynamicSqlStartColumn
                : node.StartColumn;
        }
    }
}
