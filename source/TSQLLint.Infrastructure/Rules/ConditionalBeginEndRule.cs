using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class ConditionalBeginEndRule : BaseRuleVisitor, ISqlRule
    {
        private readonly Regex IsWhiteSpaceOrSemiColon = new Regex(@"\s|;", RegexOptions.Compiled);

        public ConditionalBeginEndRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "conditional-begin-end";

        public override string RULE_TEXT => "Expected BEGIN and END statement within conditional logic block";

        public override void Visit(IfStatement node)
        {
            if (node.ThenStatement is not BeginEndBlockStatement)
            {
                errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(node), GetColumnNumber(node));
            }

            if (node.ElseStatement != null && node.ElseStatement is not BeginEndBlockStatement && node.ElseStatement is not IfStatement)
            {
                errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(node.ElseStatement), GetColumnNumber(node.ElseStatement));
            }
        }

        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
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

            var stream = statement.ScriptTokenStream;
            var indent = FixHelpers.GetIndent(fileLines, ifNode);
            var beingLine = stream[statement.FirstTokenIndex].Line - 1;
            var ifNodeLastToken = stream[statement.LastTokenIndex];
            var endLine = stream[statement.LastTokenIndex].Line;

            if (statement.StartLine == ifNodeLastToken.Line)
            {
                var index = statement.LastTokenIndex;
                actions.InsertInLine(statement.StartLine - 1, stream[index].Column, " END");
                actions.InsertInLine(statement.StartLine - 1, statement.StartColumn - 1, "BEGIN ");
            }
            else
            {
                actions.Insert(endLine, $"{indent}END");
                actions.Insert(beingLine, $"{indent}BEGIN");
            }

            static (TSqlStatement, IfStatement) FindElse(List<string> fileLines, IRuleViolation ruleViolation)
            {
                return FixHelpers.FindViolatingNode<IfStatement, TSqlStatement>(
                    fileLines, ruleViolation, x => x.ElseStatement);
            }
        }
    }
}
