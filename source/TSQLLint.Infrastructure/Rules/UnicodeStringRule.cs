using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Text;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class UnicodeStringRule : BaseRuleVisitor, ISqlRule
    {
        public UnicodeStringRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }

        public override string RULE_NAME => "unicode-string";

        public override string RULE_TEXT => "Use of unicode characters in a non unicode string";

        public override void Visit(StringLiteral node)
        {
            if (node.IsNational)
            {
                // already Unicode
                return;
            }

            if (!IsAscii(node.Value))
            {
                errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            }
        }

        private static bool IsAscii(string part)
        {
            var asciiBytes = Encoding.ASCII.GetBytes(part);
            var partAscii = Encoding.ASCII.GetString(asciiBytes);
            return part == partAscii;
        }
    }
}
