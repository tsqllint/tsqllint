using System.Collections.Generic;
using System.IO;
using TSQLLint.Lib.Parser.Interfaces;

namespace TSQLLint.Lib.Parser
{
    public interface IRuleExceptionFinder
    {
        IEnumerable<IRuleException> GetIgnoredRuleList(Stream fileStream);
    }
}
