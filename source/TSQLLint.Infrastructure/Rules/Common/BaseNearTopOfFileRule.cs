using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public abstract class BaseNearTopOfFileRule : BaseRuleVisitor, ISqlRule
    {
        private static readonly TSqlTokenType[] BeforeSet = new[] {
            TSqlTokenType.SingleLineComment,
            TSqlTokenType.MultilineComment,
            TSqlTokenType.WhiteSpace
        };

        public BaseNearTopOfFileRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public abstract string Insert { get; }
        public abstract Func<string, bool> Remove { get; }

        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            var node = FixHelpers.FindNodes<TSqlScript>(fileLines).First();

            int index;
            for (index = node.FirstTokenIndex; index <= node.LastTokenIndex; index++)
            {
                var token = node.ScriptTokenStream[index];
                var tokenType = token.TokenType;

                if (!BeforeSet.Contains(tokenType))
                {
                    break;
                }
            }

            actions.RemoveAll(Remove);
            actions.Insert(node.ScriptTokenStream[index].Line - 1, Insert);
        }
    }
}
