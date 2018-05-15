using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class NamedContraintRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public NamedContraintRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "named-constraint";

        public string RULE_TEXT => "Named constraints in temp tables can cause collisions when run in parallel";

        public int DynamicSqlStartColumn { get; set; }

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
                errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
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
    }
}
