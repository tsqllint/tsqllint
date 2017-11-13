using System;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class DataTypeLengthRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME => "data-type-length";

        public string RULE_TEXT => "Date type length not specified";

        private readonly Action<string, string, int, int> ErrorCallback;

        private readonly SqlDataTypeOption[] TypesThatRequireLength = 
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
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(SqlDataTypeReference node)
        {
            if (TypesThatRequireLength.Any(option => Equals(option, node.SqlDataTypeOption) && node.Parameters.Count < 1))
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn + node.FragmentLength);
            }
        }
    }
}
