using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class SetNoCountRule : BaseRuleVisitor, ISqlRule
    {
        public SetNoCountRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "set-nocount";

        public override string RULE_TEXT => "Expected SET NOCOUNT ON near top of file";

        public override void Visit(TSqlScript node)
        {
            var childNoCountVisitor = new ChildNoCountVisitor();
            node.AcceptChildren(childNoCountVisitor);
            if (childNoCountVisitor.NoCountIsOn)
            {
                return;
            }

            // walk child nodes to determine if rowset operations are occurring
            var childRowsetVisitor = new ChildRowsetVisitor();
            node.AcceptChildren(childRowsetVisitor);

            // walk child nodes to determine if DDL operations are occurring
            var childDDLStatementFoundVisitor = new ChildDDLStatementFoundVisitor();
            node.AcceptChildren(childDDLStatementFoundVisitor);

            if (childDDLStatementFoundVisitor.DDLStatementFound && !childRowsetVisitor.RowsetActionFound)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
        }

        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            actions.RemoveAll(x => x.StartsWith("SET NOCOUNT OFF", StringComparison.CurrentCultureIgnoreCase));
            actions.Insert(0, "SET NOCOUNT ON;");
        }

        public class ChildRowsetVisitor : TSqlFragmentVisitor
        {
            public bool RowsetActionFound { get; set; }

            public override void Visit(SelectStatement node)
            {
                RowsetActionFound = true;
            }

            public override void Visit(InsertStatement node)
            {
                RowsetActionFound = true;
            }

            public override void Visit(UpdateStatement node)
            {
                RowsetActionFound = true;
            }

            public override void Visit(DeleteStatement node)
            {
                RowsetActionFound = true;
            }
        }

        public class ChildDDLStatementFoundVisitor : TSqlFragmentVisitor
        {
            public bool DDLStatementFound { get; set; }

            public override void Visit(CreateTableStatement node)
            {
                DDLStatementFound = true;
            }

            public override void Visit(AlterTableStatement node)
            {
                DDLStatementFound = true;
            }

            public override void Visit(DropTableStatement node)
            {
                DDLStatementFound = true;
            }

            public override void Visit(TruncateTableStatement node)
            {
                DDLStatementFound = true;
            }
        }

        public class ChildNoCountVisitor : TSqlFragmentVisitor
        {
            public bool NoCountIsOn { get; private set; }

            public override void Visit(PredicateSetStatement node)
            {
                if (node.Options == SetOptions.NoCount)
                {
                    NoCountIsOn = node.IsOn;
                }
            }
        }
    }
}
