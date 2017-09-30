﻿using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Common;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class SemicolonTerminationRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME
        {
            get
            {
                return "semicolon-termination";
            }
        }

        public string RULE_TEXT
        {
            get
            {
                return "Statement not terminated with semicolon";
            }
        }

        private readonly Action<string, string, int, int> ErrorCallback;

        // don't enforce semicolon termination on these statements
        private readonly Type[] TypesToSkip = 
        {
            typeof(BeginEndBlockStatement),
            typeof(IfStatement),
            typeof(IndexDefinition),
            typeof(TryCatchStatement),
            typeof(WhileStatement)
        };

        public SemicolonTerminationRule(Action<string, string, int, int> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(TSqlStatement node)
        {
            if (TypesToSkip.Contains(node.GetType()) || EndsWithSemicolon(node))
            {
                return;
            }

            var lastToken = node.ScriptTokenStream[node.LastTokenIndex];
            var tabsOnLine = ColumnNumberCounter.CountTabsOnLine(lastToken.Line, node.LastTokenIndex, node.ScriptTokenStream);
            var column = ColumnNumberCounter.GetColumnNumberAfterToken(tabsOnLine, lastToken);
            ErrorCallback(RULE_NAME, RULE_TEXT, lastToken.Line, column);
        }

        public override void Visit(BeginEndBlockStatement node)
        {
            var beginTerminator = node.ScriptTokenStream[node.FirstTokenIndex + 1];
            if (beginTerminator.TokenType != TSqlTokenType.Semicolon)
            {
                ErrorCallback(
                    RULE_NAME, 
                    RULE_TEXT, 
                    node.ScriptTokenStream[node.FirstTokenIndex].Line, 
                    node.StartColumn + beginTerminator.Column);
            }

            if (EndsWithSemicolon(node))
            {
                return;
            }

            var endTerminator = node.ScriptTokenStream[node.LastTokenIndex];
            ErrorCallback(
                RULE_NAME,
                RULE_TEXT,
                node.ScriptTokenStream[node.LastTokenIndex].Line,
                endTerminator.Column + endTerminator.Text.Length);
        }

        private static bool EndsWithSemicolon(TSqlFragment node)
        {
            return node.ScriptTokenStream[node.LastTokenIndex].TokenType == TSqlTokenType.Semicolon
                || node.ScriptTokenStream[node.LastTokenIndex + 1].TokenType == TSqlTokenType.Semicolon;
        }
    }
}