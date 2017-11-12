namespace TSQLLint.Lib.Standard.Rules.Interface
{
    public interface ISqlRule
    {
        string RULE_NAME { get; }

        string RULE_TEXT { get; }
    }
}
