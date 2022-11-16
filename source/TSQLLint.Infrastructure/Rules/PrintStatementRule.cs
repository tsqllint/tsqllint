using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class PrintStatementRule : BaseRuleVisitor, ISqlRule
    {
        public PrintStatementRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "print-statement";

        public override string RULE_TEXT => "PRINT statement found";

        public override void Visit(PrintStatement node)
        {
            errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(node), GetColumnNumber(node));
        }
    }
}
