using TSQLLint.Common;

namespace TSQLLint.Lib.Parser.RuleExceptions
{
    public interface IExtendedRuleException : IRuleException
    {
        void SetEndLine(int endLine);
    }
}
