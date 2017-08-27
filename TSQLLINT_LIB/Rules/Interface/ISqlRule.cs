namespace TSQLLINT_LIB.Rules.Interface
{
    public interface ISqlRule
    {
        string RuleName { get; }
        string RuleText { get; }
    }
}