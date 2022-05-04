using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class InformationSchemaRule : BaseRuleVisitor, ISqlRule
    {
        public InformationSchemaRule(Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "information-schema";

        public override string RULE_TEXT => "Expected use of SYS.Partitions rather than INFORMATION_SCHEMA views";

        public override void Visit(SchemaObjectName node)
        {
            var schemaIdentifier = node.SchemaIdentifier?.Value != null;

            if (schemaIdentifier && node.SchemaIdentifier.Value.Equals("INFORMATION_SCHEMA", StringComparison.InvariantCultureIgnoreCase))
            {
                errorCallback(RULE_NAME, RULE_TEXT, node.StartLine, GetColumnNumber(node));
            }
        }

        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            var node = FixHelpers.FindNodes<TSqlStatement>(fileLines, x => x.StartLine == ruleViolation.Line);

            if (node.Count == 1)
            {
                switch (node[0])
                {
                    case IfStatement ifStatement:
                        HandleFragment(actions, ifStatement.Predicate);
                        break;

                    case SelectStatement selectStatement:
                        HandleFragment(actions, selectStatement);
                        break;
                }
            }
        }

        private static void HandleFragment(FileLineActions actions, TSqlFragment statement)
        {
            var fromClauses = FixHelpers.FindNodes<FromClause>(statement);
            var whereClauses = FixHelpers.FindNodes<WhereClause>(statement);

            if (fromClauses.Count == 1 && whereClauses.Count <= 1)
            {
                var fromClause = fromClauses[0];
                var whereClause = whereClauses.FirstOrDefault();
                var tableName = FixHelpers.FindNodes<SchemaObjectName>(fromClause)[0].BaseIdentifier.Value;
                string newFrom = null;

                switch (tableName)
                {
                    case "TABLES":
                        newFrom = "FROM sys.tables";
                        UpdateWhereTable(actions, fromClause, whereClause);

                        break;

                    case "ROUTINES":
                        newFrom = "FROM sys.procedures";
                        UpdateWhereRoutine(actions, fromClause, whereClause);
                        break;

                    case "COLUMNS":
                        newFrom = "FROM sys.columns";
                        UpdateWhereColumns(actions, fromClause, whereClause);
                        break;

                    default:
                        break;
                }

                string oldFrom = FixHelpers.GetString(fromClause);

                if (newFrom != null)
                {
                    actions.RepaceInlineAt(fromClause.StartLine - 1, fromClause.StartColumn - 1,
                    newFrom, oldFrom.Length);
                }
            }
        }

        private static string GetWhereColumnValueName(WhereClause whereClause, string columnName)
        {
            var node = FixHelpers.FindNodes<BooleanComparisonExpression>(whereClause,
                x => FixHelpers.FindNodes<Identifier>(x.FirstExpression)[0].Value == columnName);

            if (node.Count == 1 && node[0].SecondExpression is StringLiteral stringLiteral)
            {
                return stringLiteral.Value;
            }

            return null;
        }

        private static void UpdateWhere(FileLineActions actions, FromClause fromClause, WhereClause whereClause, string newWhere)
        {
            var firstWhereLine = whereClause.ScriptTokenStream[whereClause.FirstTokenIndex].Line;
            var lastWhereLine = whereClause.ScriptTokenStream[whereClause.LastTokenIndex].Line;

            // Delete mutliline where
            var anyDeleted = false;
            for (int line = lastWhereLine; line > firstWhereLine; line--)
            {
                actions.RemoveAt(line - 1);
                anyDeleted = true;
            }
            if (anyDeleted)
            {
                newWhere += ")";
            }

            var oldWhere = string.Join(string.Empty, whereClause.ScriptTokenStream
                .Where((x, i) => x.Line == firstWhereLine
                    && x.Column >= whereClause.StartColumn
                    && i <= whereClause.LastTokenIndex
                    && x.Text != "\n")
                .Select(x => x.Text));

            actions.RepaceInlineAt(whereClause.StartLine - 1, whereClause.StartColumn - 1,
                newWhere, oldWhere.Length);
        }

        private static void UpdateWhereColumns(
           FileLineActions actions,
           FromClause fromClause,
           WhereClause whereClause)
        {
            if (whereClause != null)
            {
                var schema = GetWhereColumnValueName(whereClause, "TABLE_SCHEMA");
                var columnName = GetWhereColumnValueName(whereClause, "COLUMN_NAME");
                var tableName = GetWhereColumnValueName(whereClause, "TABLE_NAME");
                var dataType = GetWhereColumnValueName(whereClause, "DATA_TYPE");

                if (schema != null && tableName != null && columnName != null)
                {
                    var newWhere = $"WHERE [object_id] = OBJECT_ID(N'{schema}.{tableName}') AND [name] = '{columnName}'";
                    if (dataType != null)
                    {
                        newWhere += $" AND [system_type_id] = TYPE_ID(N'{dataType}')";
                    }

                    UpdateWhere(actions, fromClause, whereClause, newWhere);
                }
            }
        }

        private static void UpdateWhereRoutine(
            FileLineActions actions,
            FromClause fromClause,
            WhereClause whereClause)
        {
            if (whereClause != null)
            {
                var schema = GetWhereColumnValueName(whereClause, "ROUTINE_SCHEMA");
                var name = GetWhereColumnValueName(whereClause, "ROUTINE_NAME");
                var type = GetWhereColumnValueName(whereClause, "ROUTINE_TYPE");

                if (type == "PROCEDURE" && schema != null && name != null)
                {
                    var newWhere = $"WHERE [object_id] = OBJECT_ID(N'{schema}.{name}')";

                    UpdateWhere(actions, fromClause, whereClause, newWhere);
                }
            }
        }

        private static void UpdateWhereTable(
            FileLineActions actions,
            FromClause fromClause,
            WhereClause whereClause)
        {
            if (whereClause != null)
            {
                var schema = GetWhereColumnValueName(whereClause, "TABLE_SCHEMA");
                var name = GetWhereColumnValueName(whereClause, "TABLE_NAME");
                var type = GetWhereColumnValueName(whereClause, "TABLE_TYPE");

                if (type == "BASE TABLE" && schema != null && name != null)
                {
                    var newWhere = $"WHERE [object_id] = OBJECT_ID(N'{schema}.{name}')";

                    UpdateWhere(actions, fromClause, whereClause, newWhere);
                }
            }
        }

        private int GetColumnNumber(TSqlFragment node)
        {
            return node.StartLine == DynamicSqlStartLine
                ? node.StartColumn + DynamicSqlStartColumn
                : node.StartColumn;
        }
    }
}
