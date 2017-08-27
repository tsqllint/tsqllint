using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class DataTypeLengthRule : TSqlFragmentVisitor, ISqlRule
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
        {
            ErrorCallback = errorCallback;
        }

        public string RuleName
        {
            get { return "data-type-length"; }
        }

        public string RuleText
        {
            get { return "Date type length not specified"; }
        }

        public Action<string, string, int, int> ErrorCallback { get; set; }

        public override void Visit(SqlDataTypeReference node)
        {
            for (var i = 0; i < this.typesThatRequireLength.Length; i++)
            {
                var option = this.typesThatRequireLength[i];
                if (Equals(option, node.SqlDataTypeOption) && node.Parameters.Count < 1)
                {
                    ErrorCallback(this.RuleName, this.RuleText, node.StartLine, node.StartColumn + node.FragmentLength);
                    break;
                }
            }
        }
    }
}