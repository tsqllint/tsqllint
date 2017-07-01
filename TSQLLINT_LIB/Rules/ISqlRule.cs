namespace TSQLLINT_LIB.Rules
{
    public interface ISqlRule
    {
        string RULE_NAME { get; }
        string RULE_TEXT { get; }
    }
}