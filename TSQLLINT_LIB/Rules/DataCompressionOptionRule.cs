using System;
using System.Net.NetworkInformation;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Rules.Interface;

namespace TSQLLINT_LIB.Rules
{
    public class DataCompressionOptionRule : TSqlFragmentVisitor, ISqlRule
    {
        public string RULE_NAME { get { return "data-compression"; } }
        public string RULE_TEXT { get { return "All Table and indexes including, Temp tables, should be compressed appropriately"; } }
        public Action<string, string, TSqlFragment> ErrorCallback;

        public DataCompressionOptionRule(Action<string, string, TSqlFragment> errorCallback)
        {
            ErrorCallback = errorCallback;
        }

        public override void Visit(CreateTableStatement node)
        {
            var compressionOptionExists = false;
            for (var index = 0; index < node.Options.Count; index++)
            {
                var tableOption = node.Options[index];
                if (tableOption.OptionKind == TableOptionKind.DataCompression)
                {
                    compressionOptionExists = true;
                }
            }

            for (var index = 0; index < node.Definition.TableConstraints.Count; index++)
            {
                var constraint = node.Definition.TableConstraints[index];
                if (constraint.GetType() != typeof(UniqueConstraintDefinition))
                {
                    continue;
                }

                var tableConstraint = (UniqueConstraintDefinition) constraint;
                for (var i = 0; i < tableConstraint.IndexOptions.Count; i++)
                {
                    var indexOption = tableConstraint.IndexOptions[i];
                    if (indexOption.OptionKind == IndexOptionKind.DataCompression)
                    {
                        compressionOptionExists = true;
                    }
                }
            }

            if (!compressionOptionExists)
            {
                ErrorCallback(RULE_NAME, RULE_TEXT, node);
            }
        }
    }
}