using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class SetVariableRule : BaseRuleVisitor, ISqlRule
    {
        private const string SELECT = "SELECT";
        private const string SET = "SET";
        private const int SET_LENGTH = 3;

        public SetVariableRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "set-variable";

        public override string RULE_TEXT => "Expected variable to be set using SELECT statement";

        public override void Visit(SetVariableStatement node)
        {
            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, GetColumnNumber(node));
        }

        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            var lineIndex = ruleViolation.Line - 1;
            var columnIndex = FixHelpers.GetIndent(fileLines, ruleViolation).Length;
            var expectedSet = fileLines[lineIndex].Substring(columnIndex, SET_LENGTH);

            if (string.Compare(expectedSet, SET, true) == 0)
            {
                actions.RepaceInlineAt(lineIndex, columnIndex, SELECT, SET_LENGTH);
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
