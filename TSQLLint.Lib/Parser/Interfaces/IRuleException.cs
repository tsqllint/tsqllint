namespace TSQLLint.Lib.Standard.Parser.Interfaces
{
    public interface IRuleException
    {
        int EndLine { get; }
        
        int StartLine { get; }

        void SetEndLine(int endLine);
    }
}
