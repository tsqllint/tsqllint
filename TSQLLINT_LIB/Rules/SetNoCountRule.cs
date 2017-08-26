using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SetNoCountRule : TSqlFragmentVisitor, ISqlRule
    {
        public SetNoCountRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "set-nocount"; }
        }

        public string RuleText
        {
            get { return "Expected SET NOCOUNT ON near top of file"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(TSqlScript node)
        {
            var childNoCountVisitor = new ChildNoCountVisitor();
            node.AcceptChildren(childNoCountVisitor);
            if (childNoCountVisitor.SetNoCountFound)
            {
                return;
            }

            // walk child nodes to determine if rowset operations are occurring
            var childRowsetVisitor = new ChildRowsetVisitor();
            node.AcceptChildren(childRowsetVisitor);

            // walk child nodes to determine if DDL operations are occurring
            var childDDLStatementFoundVisitor = new ChildDDLStatementFoundVisitor();
            node.AcceptChildren(childDDLStatementFoundVisitor);

            if (!childNoCountVisitor.SetNoCountFound && 
                !childDDLStatementFoundVisitor.DDLStatementFound && 
                !childRowsetVisitor.RowsetActionFound)
            {
                return;
            }

            ErrorCallback(RuleName, RuleText, node.StartLine, node.StartColumn);
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
            public bool SetNoCountFound { get; set; }

            public override void Visit(SetOnOffStatement node)
            {
                var typedNode = node as PredicateSetStatement;
                if (typedNode != null && typedNode.Options == SetOptions.NoCount)
                {
                    SetNoCountFound = true;
                }
            }
        }
    }
}