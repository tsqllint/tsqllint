using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class PrintStatementRule : TSqlFragmentVisitor, ISqlRule
    {
        public PrintStatementRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "print-statement"; }
        }

        public string RuleText
        {
            get { return "PRINT statement found"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(PrintStatement node)
        {
            ErrorCallback(this.RuleName, this.RuleText, node.StartLine, node.StartColumn);
        }
    }
}
