using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class DuplicateEmptyLineRule : BaseRuleVisitor, ISqlRule
    {
        private static readonly Regex EmptyLineRegex = new(@"^\s*$", RegexOptions.Compiled);

        public DuplicateEmptyLineRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "duplicate-empty-line";

        public override string RULE_TEXT => "Duplicate new line found";

        public override void Visit(TSqlScript node)
        {
            var isEmptyLine = false;
            var fileLines = FixHelpers.GetString(node)
                .Split('\n')
                .ToList();

            for (var i = 0; i < fileLines.Count; i++)
            {
                if (EmptyLineRegex.IsMatch(fileLines[i]))
                {
                    if (isEmptyLine)
                    {
                        errorCallback(RULE_NAME, RULE_TEXT, i + 1, 1);
                    }

                    isEmptyLine = true;
                }
                else
                {
                    isEmptyLine = false;
                }
            }
        }

        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            actions.RemoveAt(ruleViolation.Line - 1);
        }
    }
}
