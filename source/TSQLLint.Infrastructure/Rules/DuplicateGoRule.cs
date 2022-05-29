using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class DuplicateGoRule : BaseRuleVisitor, ISqlRule
    {
        public DuplicateGoRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "duplicate-go";

        public override string RULE_TEXT => "Duplicate GO statement found";

        public override void Visit(TSqlScript node)
        {
            TSqlParserToken lastToken = null;
            TSqlParserToken currentToken;
            for (var index = 0; index <= node.LastTokenIndex; index++)
            {
                var tokenType = node.ScriptTokenStream[index].TokenType;

                // Skip these
                switch (tokenType)
                {
                    case TSqlTokenType.MultilineComment:
                    case TSqlTokenType.WhiteSpace:
                    case TSqlTokenType.Semicolon:
                        continue;
                }

                currentToken = node.ScriptTokenStream[index];

                if (tokenType is TSqlTokenType.Go &&
                   lastToken?.TokenType is TSqlTokenType.Go)
                {
                    errorCallback(RULE_NAME, RULE_TEXT, currentToken.Line, currentToken.Column);
                }

                lastToken = currentToken;
            }
        }

        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            actions.RemoveAt(ruleViolation.Line - 1);
        }
    }
}
