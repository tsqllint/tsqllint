using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class SetVariableRule : BaseRuleVisitor, ISqlRule
    {
        private const string SELECT = "SELECT";
        private readonly Regex SetRegex = new Regex(@"\sSET\s|^SET\s", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private const int SET_LENGTH = 3;

        public SetVariableRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "set-variable";

        public override string RULE_TEXT => "Expected variable to be set using SELECT statement";

        public override void Visit(SetVariableStatement node)
        {
            errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(node), GetColumnNumber(node));
        }

        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            var lineIndex = ruleViolation.Line - 1;
            var line = fileLines[ruleViolation.Line - 1];
            var regex = SetRegex.Match(line);

            if (regex.Success)
            {
                var isStartOfLine = regex.ValueSpan.Length <= SET_LENGTH + 1;
                actions.RepaceInlineAt(lineIndex, isStartOfLine ? regex.Index : regex.Index + 1, SELECT, SET_LENGTH);
            }
        }
    }
}
