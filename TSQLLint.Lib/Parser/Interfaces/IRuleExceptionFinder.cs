using System.Collections.Generic;
using System.IO;
using TSQLLint.Common;

namespace TSQLLint.Lib.Parser.Interfaces
{
    public interface IRuleExceptionFinder
    {
        IEnumerable<IRuleException> GetIgnoredRuleList(Stream fileStream);
    }
}
