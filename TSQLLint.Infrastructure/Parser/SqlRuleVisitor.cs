using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Configuration.Overrides;
using TSQLLint.Infrastructure.Interfaces;
using TSQLLint.Infrastructure.Rules.RuleExceptions;
using TSQLLint.Infrastructure.Rules.RuleViolations;

namespace TSQLLint.Infrastructure.Parser
{
    public class SqlRuleVisitor : IRuleVisitor
    {
        private readonly IFragmentBuilder fragmentBuilder;

        private readonly IRuleVisitorBuilder ruleVisitorBuilder;

        private readonly IReporter reporter;

        private readonly OverrideFinder overrideFinder = new OverrideFinder();

        public SqlRuleVisitor(IRuleVisitorBuilder ruleVisitorBuilder, IFragmentBuilder fragmentBuilder, IReporter reporter)
        {
            this.fragmentBuilder = fragmentBuilder;
            this.reporter = reporter;
            this.ruleVisitorBuilder = ruleVisitorBuilder;
        }

        public void VisitRules(string sqlPath, IEnumerable<IRuleException> ignoredRules, Stream sqlFileStream)
        {
            var overrides = overrideFinder.GetOverrideList(sqlFileStream);
            var overrideArray = overrides as IOverride[] ?? overrides.ToArray();

            var sqlFragment = fragmentBuilder.GetFragment(GetSqlTextReader(sqlFileStream), out var errors, overrideArray);
            
            // notify user of syntax errors
            var ruleExceptions = ignoredRules as IRuleException[] ?? ignoredRules.ToArray();
            if (errors.Any())
            {
                HandleParserErrors(sqlPath, errors, ruleExceptions);
            }

            // lint what can be linted, even if there are errors
            var ruleVisitors = ruleVisitorBuilder.BuildVisitors(sqlPath, ruleExceptions);
            foreach (var visitor in ruleVisitors)
            {
                VisitFragment(sqlFragment, visitor, overrideArray);
            }
        }

        private void VisitFragment(TSqlFragment sqlFragment, TSqlFragmentVisitor visitor, IEnumerable<IOverride> overrides)
        {
            sqlFragment?.Accept(visitor);

            if (!VisitorIsBlackListedForDynamicSql(visitor))
            {
                var dynamicSqlVisitor = new DynamicSQLParser(DynamicSqlCallback);
                sqlFragment?.Accept(dynamicSqlVisitor);
            }

            void DynamicSqlCallback(string dynamicSQL, int DynamicSqlStartColumn)
            {
                ((ISqlRule)visitor).DynamicSqlStartColumn = DynamicSqlStartColumn;
                var dynamicSqlStream = ParsingUtility.GenerateStreamFromString(dynamicSQL);
                var dynamicFragment = fragmentBuilder.GetFragment(GetSqlTextReader(dynamicSqlStream), out var errors, overrides);
                dynamicFragment?.Accept(visitor);
            }
        }

        private static bool VisitorIsBlackListedForDynamicSql(TSqlFragmentVisitor visitor)
        {
            if (visitor == null)
            {
                return false;
            }
            
            return new List<string>
            {
                "SetAnsiNullsRule",
                "SetNoCountRule",
                "SetQuotedIdentifierRule",
                "SetTransactionIsolationLevelRule"
            }.Any(x => visitor.GetType().ToString().Contains(x));
        }

        private static StreamReader GetSqlTextReader(Stream sqlFileStream)
        {
            var sqlText = new StreamReader(sqlFileStream);
            sqlFileStream.Seek(0, SeekOrigin.Begin);
            return sqlText;
        }

        private void HandleParserErrors(string sqlPath, IEnumerable<ParseError> errors, IEnumerable<IRuleException> ignoredRules)
        {
            var updatedExitCode = false;
            var ruleExceptions = ignoredRules as IRuleException[] ?? ignoredRules.ToArray();

            foreach (var error in errors)
            {
                var globalRulesOnLine = ruleExceptions.OfType<GlobalRuleException>().Where(
                    x => error.Line >= x.StartLine
                    && error.Line <= x.EndLine);

                if (!globalRulesOnLine.Any())
                {
                    reporter.ReportViolation(new RuleViolation(sqlPath, "invalid-syntax", error.Message, error.Line, error.Column, RuleViolationSeverity.Error));
                    if (updatedExitCode)
                    {
                        continue;
                    }

                    updatedExitCode = true;
                    Environment.ExitCode = 1;
                }
            }
        }
    }
}
