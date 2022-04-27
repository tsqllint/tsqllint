using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class CaseSensitiveVariablesRule : BaseRuleVisitor
    {
        private readonly List<string> variableNames;

        public CaseSensitiveVariablesRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
            this.variableNames = new List<string>();
        }

        public override string RULE_NAME => "case-sensitive-variables";

        public override string RULE_TEXT => "Expected variable names to use common casing";

        public override void Visit(DeclareVariableStatement node)
        {
            foreach (var decalaration in node.Declarations)
            {
                variableNames.Add(decalaration.VariableName.Value);
            }
        }

        public override void Visit(VariableReference node)
        {
            var variableName = node.Name;
            var caseInsensitiveMatch = variableNames.Where(v => v.ToUpper() == variableName.ToUpper());

            if (!caseInsensitiveMatch.Any() || caseInsensitiveMatch.First() == variableName)
            {
                return;
            }

            errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, GetColumnNumber(node));
        }

        private int GetColumnNumber(TSqlFragment node)
        {
            return node.StartLine == DynamicSqlStartLine
                ? node.StartColumn + DynamicSqlStartColumn
                : node.StartColumn;
        }
    }
}
