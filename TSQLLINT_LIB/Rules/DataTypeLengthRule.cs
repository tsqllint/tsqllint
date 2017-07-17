using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class DataTypeLengthRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "data-type-length"; } }
        public string RULE_TEXT { get { return "Date type length must be specified"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        private readonly SqlDataTypeOption[] TypesThatRequireLength = {
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

        public DataTypeLengthRule(Action<string, string, TSqlFragment> errorCallback)
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
                    ErrorCallback(RULE_NAME, RULE_TEXT, node);
                    break;
                }
            }
        }
    }
}