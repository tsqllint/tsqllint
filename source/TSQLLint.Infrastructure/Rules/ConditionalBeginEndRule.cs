using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Helpers;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class ConditionalBeginEndRule : BaseRuleVisitor, ISqlRule
    {
        public ConditionalBeginEndRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "conditional-begin-end";

        public override string RULE_TEXT => "Expected BEGIN and END statement within conditional logic block";

        public override void Visit(IfStatement node)
        {
            Foo(node);

            if (node.ElseStatement != null)
            {
                Foo(node.ElseStatement);
            }
        }

        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation)
        {
            var ifNode = FixHelpers.FindViolatingNode<IfStatement>(fileLines, ruleViolation);
            TSqlStatement statement;

            if (ifNode == null)
            {
                (statement, ifNode) = FindElse(fileLines, ruleViolation);
            }
            else
            {
                statement = ifNode.ThenStatement;
            }

            var indent = FixHelpers.GetIndent(fileLines, ifNode);
            var beingLine = statement.ScriptTokenStream[statement.FirstTokenIndex].Line - 1;
            var endLine = statement.ScriptTokenStream[statement.LastTokenIndex].Line;

            fileLines.Insert(endLine, $"{indent}END");
            fileLines.Insert(beingLine, $"{indent}BEGIN");

            static (TSqlStatement, IfStatement) FindElse(List<string> fileLines, IRuleViolation ruleViolation)
            {
                return FixHelpers.FindViolatingNode<IfStatement, TSqlStatement>(
                    fileLines, ruleViolation, x => x.ElseStatement);
            }
        }

        private void Foo(TSqlFragment node)
        {
            var childBeginEndVisitor = new ChildBeginEndVisitor();
            node.AcceptChildren(childBeginEndVisitor);

            if (childBeginEndVisitor.BeginEndBlockFound ||
                node is BeginEndBlockStatement)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, GetColumnNumber(node));
        }

        private class ChildBeginEndVisitor : TSqlFragmentVisitor
        {
            public bool BeginEndBlockFound
            {
                get;
                private set;
            }

            public override void Visit(BeginEndBlockStatement node)
            {
                BeginEndBlockFound = true;
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
