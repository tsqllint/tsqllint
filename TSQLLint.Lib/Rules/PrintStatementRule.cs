using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Standard.Rules.Interface;

namespace TSQLLint.Lib.Standard.Rules
{
    public class PrintStatementRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "print-statement";

        public string RULE_TEXT => "PRINT statement found";

        private readonly Action<string, string, int, int> ErrorCallback;

        public PrintStatementRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(PrintStatement node)
        {
            ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
        }
    }
}
