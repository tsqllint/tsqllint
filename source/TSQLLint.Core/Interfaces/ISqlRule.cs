using TSQLLint.Common;

namespace TSQLLint.Core.Interfaces
{
    public interface ISqlRule : ISqlLintRule
    {
        string RULE_TEXT { get; }

        int DynamicSqlStartColumn { get; set; }

        int DynamicSqlStartLine { get; set; }
    }
}
