using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class PrintStatementRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME {get { return "print-statement";}}
        public string RULE_TEXT { get { return "Do not put print statements in scripts."; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public PrintStatementRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(PrintStatement node)
        {
            ErrorCallback(RULE_NAME, RULE_TEXT, node);
        }
    }
}
