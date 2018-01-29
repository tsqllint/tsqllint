using System.Collections.Generic;
using System.IO;
using TSQLLint.Lib.Parser.RuleExceptions;

namespace TSQLLint.Lib.Parser.Interfaces
{
    public interface IRuleExceptionFinder
    {
        IEnumerable<IExtendedRuleException> GetIgnoredRuleList(Stream fileStream);
    }
}
