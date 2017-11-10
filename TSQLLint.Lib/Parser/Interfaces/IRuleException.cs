namespace TSQLLint.Lib.Parser
{
    public interface IRuleException
    {
        int EndLine { get; }
        
        int StartLine { get; }

        void SetEndLine(int endLine);
    }
}
