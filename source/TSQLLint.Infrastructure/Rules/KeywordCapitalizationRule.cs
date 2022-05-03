using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TSQLLint.Common;
using TSQLLint.Core;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class KeywordCapitalizationRule : BaseRuleVisitor, ISqlRule
    {
        public KeywordCapitalizationRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "keyword-capitalization";

        public override string RULE_TEXT => "Expected TSQL Keyword to be capitalized";

        public override void Visit(TSqlScript node)
        {
            var typesToUpcase = Constants.TSqlKeywords.Concat(Constants.TSqlDataTypes).ToArray();
            for (var index = 0; index < node.ScriptTokenStream?.Count; index++)
            {
                var token = node.ScriptTokenStream[index];
                if (!typesToUpcase.Contains(token.Text, StringComparer.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                if (IsUpperCase(token.Text))
                {
                    continue;
                }

                var dynamicSQLAdjustment = AdjustColumnForDymamicSQL(token);

                // get a count of all tabs on the line that occur prior to the last token in this node
                var tabsOnLine = ColumnNumberCalculator.CountTabsBeforeToken(token.Line, index, node.ScriptTokenStream);
                var column = ColumnNumberCalculator.GetColumnNumberBeforeToken(tabsOnLine, token);

                errorCallback(RULE_NAME, RULE_TEXT, token.Line, column + dynamicSQLAdjustment);
            }
        }

        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            var lineIndex = ruleViolation.Line - 1;
            var line = fileLines[lineIndex];
            var startCharIndex = ColumnNumberCalculator.GetIndex(line, ruleViolation.Column);

            var errorWord = new Regex(@"\w+").Matches(line[startCharIndex..]).First().Value;

            actions.RemoveInLine(lineIndex, startCharIndex, errorWord.Length);
            actions.InsertInLine(lineIndex, startCharIndex, errorWord.ToUpper());
        }

        private int AdjustColumnForDymamicSQL(TSqlParserToken node)
        {
            return node.Line == DynamicSqlStartLine
                ? DynamicSqlStartColumn
                : 0;
        }

        private static bool IsUpperCase(string input)
        {
            return input.All(t => !char.IsLetter(t) || char.IsUpper(t));
        }
    }
}
