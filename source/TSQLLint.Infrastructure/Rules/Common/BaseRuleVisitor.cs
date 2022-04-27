using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules.Common
{
    public abstract class BaseRuleVisitor : TSqlFragmentVisitor, ISqlRule
    {
        protected readonly Action<string, string, int, int> errorCallback;

        public BaseRuleVisitor(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public int DynamicSqlStartColumn { get; set; }
        public int DynamicSqlStartLine { get; set; }
        public abstract string RULE_NAME { get; }
        public abstract string RULE_TEXT { get; }

        public virtual async Task FixViolations(IList<IRuleViolation> ruleViolations)
        {
            await Task.Yield();
        }

        protected async Task FixViolations(
            IList<IRuleViolation> ruleViolations, Action<string[], IRuleViolation> fix)
        {
            await Task.Yield();

            var files = ruleViolations.GroupBy(x => x.FileName);

            foreach (var file in files)
            {
                var fileLines = await File.ReadAllLinesAsync(file.Key);

                foreach (var ruleViolation in ruleViolations
                    .OrderByDescending(x => x.Line)
                    .ThenByDescending(x => x.Column))
                {
                    fix(fileLines, ruleViolation);
                }

                await File.WriteAllLinesAsync(file.Key, fileLines);
            }
        }
    }
}
