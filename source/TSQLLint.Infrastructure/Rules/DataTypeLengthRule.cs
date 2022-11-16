using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Linq;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class DataTypeLengthRule : BaseRuleVisitor, ISqlRule
    {
        private readonly SqlDataTypeOption[] typesThatRequireLength =
        {
                SqlDataTypeOption.Char,
                SqlDataTypeOption.VarChar,
                SqlDataTypeOption.NVarChar,
                SqlDataTypeOption.NChar,
                SqlDataTypeOption.Binary,
                SqlDataTypeOption.VarBinary,
                SqlDataTypeOption.Decimal,
                SqlDataTypeOption.Numeric,
                SqlDataTypeOption.Float
        };

        public DataTypeLengthRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "data-type-length";

        public override string RULE_TEXT => "Data type length not specified";

        public override void Visit(SqlDataTypeReference node)
        {
            if (typesThatRequireLength.Any(option => Equals(option, node.SqlDataTypeOption) && node.Parameters.Count < 1))
            {
                errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(node), GetColumnNumber(node));
            }
        }

        protected override int GetColumnNumber(TSqlFragment node)
        {
            return node.FragmentLength + base.GetColumnNumber(node);
        }
    }
}
