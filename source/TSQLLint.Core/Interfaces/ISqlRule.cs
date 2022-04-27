using System.Collections.Generic;
using System.Threading.Tasks;
using TSQLLint.Common;

namespace TSQLLint.Core.Interfaces
{
    public interface ISqlRule
    {
        string RULE_NAME { get; }

        string RULE_TEXT { get; }

        int DynamicSqlStartColumn { get; set; }

        int DynamicSqlStartLine { get; set; }

        Task FixViolations(IList<IRuleViolation> ruleViolations);
    }
}
