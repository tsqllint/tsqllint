﻿using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Common;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class KeywordCapitalizationRule : TSqlFragmentVisitor, ISqlRule
    {
        public KeywordCapitalizationRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "keyword-capitalization"; }
        }

        public string RuleText
        {
            get { return "Expected TSQL Keyword to be capitalized"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(TSqlScript node)
        {
            for (var index = 0; index < node.ScriptTokenStream.Count; index++)
            {
                var token = node.ScriptTokenStream[index];
                if (!Constants.TSqlKeywords.Contains(token.Text, StringComparer.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                if (IsAllUpper(token.Text))
                {
                    continue;
                }

                // get a count of all tabs on the line that occur prior to the last token in this node
                var tabsOnLine = ColumnNumberCounter.CountTabsOnLine(token.Line, index, node.ScriptTokenStream);
                var column = ColumnNumberCounter.GetColumnNumberBeforeToken(tabsOnLine, token);

                ErrorCallback(RuleName, RuleText, token.Line, column);
            }
        }

        private static bool IsAllUpper(string input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                if (!char.IsUpper(input[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}