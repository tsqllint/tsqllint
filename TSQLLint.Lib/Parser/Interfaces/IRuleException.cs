namespace TSQLLint.Lib.Parser.Interfaces
{
    public interface IRuleException
    {
        int EndLine { get; }
        
        int StartLine { get; }

        void SetEndLine(int endLine);
    }
}
