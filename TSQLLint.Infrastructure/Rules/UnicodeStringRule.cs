using System;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules
{
    public class UnicodeStringRule : TSqlFragmentVisitor, ISqlRule
    {
        private readonly Action<string, string, int, int> errorCallback;

        public UnicodeStringRule(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public string RULE_NAME => "unicode-string";

        public string RULE_TEXT => "Use of unicode characters in a non unicode string";

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
