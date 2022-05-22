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
    public class SemicolonTerminationRule : BaseRuleVisitor, ISqlRule
    {
        private readonly IList<TSqlFragment> waitForStatements = new List<TSqlFragment>();
        private static Regex WhiteSpaceRegex = new Regex(@"\s", RegexOptions.Compiled);
        private static Regex AllWhiteSpaceRegex = new Regex(@"^\s$", RegexOptions.Compiled);

        // don't enforce semicolon termination on these statements
        private readonly Type[] typesToSkip =
        {
            typeof(BeginEndBlockStatement),
            typeof(GoToStatement),
            typeof(IndexDefinition),
            typeof(LabelStatement),
            typeof(WhileStatement),
            typeof(IfStatement),
            typeof(CreateViewStatement)
        };

        public SemicolonTerminationRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "semicolon-termination";

        public override string RULE_TEXT => "Statement not terminated with semicolon";

        public override void Visit(WaitForStatement node)
        {
            waitForStatements.Add(node.Statement);
        }

        public override void Visit(TSqlStatement node)
        {
            if (Array.IndexOf(typesToSkip, node.GetType()) > -1 ||
                EndsWithSemicolon(node) ||
                waitForStatements.Contains(node))
            {
                return;
            }

            var dynamicSqlColumnOffset = node.StartLine == DynamicSqlStartLine
                ? DynamicSqlStartColumn
                : 0;

            var (lastToken, column) = GetLastTokenAndColumn(node);
            errorCallback(RULE_NAME, RULE_TEXT, lastToken.Line, column + dynamicSqlColumnOffset);
        }

        private static (TSqlParserToken, int) GetLastTokenAndColumn(TSqlStatement node)
        {
            var lastToken = node.ScriptTokenStream[node.LastTokenIndex];
            var tabsOnLine = ColumnNumberCalculator.CountTabsBeforeToken(lastToken.Line, node.LastTokenIndex, node.ScriptTokenStream);
            var column = ColumnNumberCalculator.GetColumnNumberAfterToken(tabsOnLine, lastToken);

            return (lastToken, column);
        }

        private static bool EndsWithSemicolon(TSqlFragment node)
        {
            return node.ScriptTokenStream[node.LastTokenIndex].TokenType == TSqlTokenType.Semicolon
                || node.ScriptTokenStream[node.LastTokenIndex + 1].TokenType == TSqlTokenType.Semicolon;
        }

        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            var lineIndex = ruleViolation.Line - 1;
            var line = fileLines[lineIndex];
            var node = FixHelpers.FindNodes<TSqlStatement>(fileLines, x =>
            {
                var (lastToken, column) = GetLastTokenAndColumn(x);
                return lastToken.Line == ruleViolation.Line && column == ruleViolation.Column;
            }).FirstOrDefault();

            if (node != null)
            {
                var (lastToken, column) = GetLastTokenAndColumn(node);
                var index = lastToken.Column + lastToken.Text.Length;

                if (index > 1)
                {
                    actions.InsertInLine(lastToken.Line - 1, index - 1, ";");
                }
            }
        }
    }
}
