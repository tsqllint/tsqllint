namespace TSQLLint.Lib.Rules.Interface
{
    public interface ISqlRule
    {
        string RULE_NAME { get; }

        string RULE_TEXT { get; }
    }
}