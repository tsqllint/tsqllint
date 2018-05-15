using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class SemicolonTerminationRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        private readonly IList<TSqlFragment> waitForStatements = new List<TSqlFragment>();

        // don't enforce semicolon termination on these statements
        private readonly Type[] typesToSkip =
        {
            typeof(BeginEndBlockStatement),
            typeof(GoToStatement),
            typeof(IndexDefinition),
            typeof(LabelStatement),
            typeof(WhileStatement),
            typeof(IfStatement)
        };

        public SemicolonTerminationRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "semicolon-termination";

        public string RULE_TEXT => "Statement not terminated with semicolon";

        public int DynamicSqlStartColumn { get; set; }

        public override void Visit(WaitForStatement node)
        {
            waitForStatements.Add(node.Statement);
        }

        public override void Visit(TSqlStatement node)
        {
            if (typesToSkip.Contains(node.GetType()) ||
                EndsWithSemicolon(node) ||
                waitForStatements.Contains(node))
            {
                return;
            }

            var lastToken = node.ScriptTokenStream[node.LastTokenIndex];
            var tabsOnLine = ColumnNumberCalculator.CountTabsBeforeToken(lastToken.Line, node.LastTokenIndex, node.ScriptTokenStream);
            var column = ColumnNumberCalculator.GetColumnNumberAfterToken(tabsOnLine, lastToken);
            errorCallback(RULE_NAME, RULE_TEXT, lastToken.Line, column + DynamicSqlStartColumn);
        }

        private static bool EndsWithSemicolon(TSqlFragment node)
        {
            return node.ScriptTokenStream[node.LastTokenIndex].TokenType == TSqlTokenType.Semicolon
                || node.ScriptTokenStream[node.LastTokenIndex + 1].TokenType == TSqlTokenType.Semicolon;
        }
    }
}
