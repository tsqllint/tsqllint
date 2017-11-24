using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class FullTextRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "full-text";

        public string RULE_TEXT => "Full text operation found, this can cause performance problems";

        private readonly Action<string, string, int, int> ErrorCallback;

        public FullTextRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(FunctionCall node)
        {
            if (node.FunctionName.Value.Equals("CONTAINS", StringComparison.OrdinalIgnoreCase) ||
                node.FunctionName.Value.Equals("CONTAINSTABLE", StringComparison.OrdinalIgnoreCase) ||
                node.FunctionName.Value.Equals("FREETEXT", StringComparison.OrdinalIgnoreCase) ||
                node.FunctionName.Value.Equals("FREETEXTTABLE", StringComparison.OrdinalIgnoreCase))
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            }
        }
    }
}
