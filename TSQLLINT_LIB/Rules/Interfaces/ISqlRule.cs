namespace TSQLLINT_LIB.Rules.Interfaces
{
    internal interface ISqlRule
    {
        string RULE_NAME { get; }
        string RULE_TEXT { get; }
    }
}