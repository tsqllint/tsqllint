using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class SemicolonTerminationRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        private readonly IList<TSqlFragment> waitForStatements = new List<TSqlFragment>();

        // don't enforce semicolon termination on these statements
        private readonly Type[] typesToSkip =
        {
            typeof(BeginEndBlockStatement),
            typeof(GoToStatement),
            typeof(IndexDefinition),
            typeof(LabelStatement),
            typeof(WhileStatement),
            typeof(IfStatement),
            typeof(CreateViewStatement)
        };

        public SemicolonTerminationRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "semicolon-termination";

        public string RULE_TEXT => "Statement not terminated with semicolon";

        public int DynamicSqlStartColumn { get; set; }

        public int DynamicSqlStartLine { get; set; }

        public override void Visit(WaitForStatement node)
        {
            waitForStatements.Add(node.Statement);
        }

        public override void Visit(CreateFunctionStatement node)
        {
            // if this node is create function statement, its return statement's child nodes should not be terminated by semicolon
            // otherwise it produces a syntax error
            var childReturnVisitor = new ChildReturnVisitor();
            node.AcceptChildren(childReturnVisitor);
            foreach (SelectFunctionReturnType childReturnNode in childReturnVisitor.ReturnNodes)
            {
                var childSelectVisitor = new ChildSelectVisitor();
                childReturnNode.AcceptChildren(childSelectVisitor);
                foreach (TSqlFragment childSelectNode in childSelectVisitor.SelectNodes)
                {
                    waitForStatements.Add(childSelectNode);
                }
            }
        }

        public override void Visit(TSqlStatement node)
        {
            if (Array.IndexOf(typesToSkip, node.GetType()) > -1 ||
                EndsWithSemicolon(node) ||
                waitForStatements.Contains(node))
            {
                return;
            }

            var dynamicSqlColumnOffset = node.StartLine == DynamicSqlStartLine
                ? DynamicSqlStartColumn
                : 0;

            var lastToken = node.ScriptTokenStream[node.LastTokenIndex];
            var tabsOnLine = ColumnNumberCalculator.CountTabsBeforeToken(lastToken.Line, node.LastTokenIndex, node.ScriptTokenStream);
            var column = ColumnNumberCalculator.GetColumnNumberAfterToken(tabsOnLine, lastToken);
            errorCallback(RULE_NAME, RULE_TEXT, lastToken.Line, column + dynamicSqlColumnOffset);
        }

        private static bool EndsWithSemicolon(TSqlFragment node)
        {
            return node.ScriptTokenStream[node.LastTokenIndex].TokenType == TSqlTokenType.Semicolon
                || node.ScriptTokenStream[node.LastTokenIndex + 1].TokenType == TSqlTokenType.Semicolon;
        }

        public class ChildReturnVisitor : TSqlFragmentVisitor
        {
            public HashSet<SelectFunctionReturnType> ReturnNodes { get; } = new HashSet<SelectFunctionReturnType>();

            public override void Visit(SelectFunctionReturnType node)
            {
                ReturnNodes.Add(node);
            }
        }

        public class ChildSelectVisitor : TSqlFragmentVisitor
        {
            public HashSet<SelectStatement> SelectNodes { get; } = new HashSet<SelectStatement>();

            public override void Visit(SelectStatement node)
            {
                SelectNodes.Add(node);
            }
        }
    }
}
