using TSQLLint.Common;

namespace TSQLLint.Core.Interfaces
{
    public interface IExtendedRuleException : IRuleException
    {
        void SetEndLine(int endLine);
    }
}
