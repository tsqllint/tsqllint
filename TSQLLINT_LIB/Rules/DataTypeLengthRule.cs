using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class DataTypeLengthRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "data-type-length"; } }
        public string RULE_TEXT { get { return "Type length must be specified"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        private readonly SqlDataTypeOption[] RequireLength = {
                SqlDataTypeOption.Char,
                SqlDataTypeOption.VarChar,
                SqlDataTypeOption.NVarChar,
                SqlDataTypeOption.NChar,
                SqlDataTypeOption.Binary,
                SqlDataTypeOption.VarBinary,
                SqlDataTypeOption.Decimal,
                SqlDataTypeOption.Numeric,
                SqlDataTypeOption.Float,
                SqlDataTypeOption.DateTime,
                SqlDataTypeOption.DateTimeOffset
            };


        public DataTypeLengthRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(SqlDataTypeReference node)
        {
            for (var i = 0; i < RequireLength.Length; i++)
            {
                var option = RequireLength[i];
                if (Equals(option, node.SqlDataTypeOption) && node.Parameters.Count < 1)
                {
                    ErrorCallback(RULE_NAME, RULE_TEXT, node);
                    break;
                }
            }
        }
    }
}