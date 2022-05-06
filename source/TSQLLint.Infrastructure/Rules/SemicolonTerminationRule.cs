using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class SemicolonTerminationRule : BaseRuleVisitor, ISqlRule
    {
        private readonly IList<TSqlFragment> waitForStatements = new List<TSqlFragment>();

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

            var lastToken = node.ScriptTokenStream[node.LastTokenIndex];
            var tabsOnLine = ColumnNumberCalculator.CountTabsBeforeToken(lastToken.Line, node.LastTokenIndex, node.ScriptTokenStream);
            var column = ColumnNumberCalculator.GetColumnNumberAfterToken(tabsOnLine, lastToken);
            errorCallback(RULE_NAME, RULE_TEXT, lastToken.Line, column + dynamicSqlColumnOffset);
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
            var charIndex = line.IndexOf("--");

            if (charIndex == -1)
            {
                actions.UpdateLine(lineIndex, $"{fileLines[lineIndex].TrimEnd()};");
            }
            else
            {
                charIndex--;

                while (charIndex >= 0 && new Regex(@"\s").IsMatch(line[charIndex].ToString()))
                {
                    charIndex--;
                    break;
                }

                actions.InsertInLine(lineIndex, charIndex + 1, ";");
            }
        }
    }
}
