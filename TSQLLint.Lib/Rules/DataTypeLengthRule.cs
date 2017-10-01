using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.Interface;

namespace TSQLLint.Lib.Rules
{
    public class DataTypeLengthRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME
        {
            get
            {
                return "data-type-length";
            }
        }

        public string RULE_TEXT
        {
            get
            {
                return "Date type length not specified";
            }
        }

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
            for (var i = 0; i < TypesThatRequireLength.Length; i++)
            {
                var option = TypesThatRequireLength[i];
                if (Equals(option, node.SqlDataTypeOption) && node.Parameters.Count < 1)
                {
                    ErrorCallback(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn + node.FragmentLength);
                    break;
                }
            }
        }
    }
}