using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Common;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class FullTextRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "full-text";

        public string RULE_TEXT => "Full text predicate found, this can cause performance problems";

        private readonly Action<string, string, int, int> ErrorCallback;

        public FullTextRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(FullTextPredicate node){
            ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
        }
    }
}
