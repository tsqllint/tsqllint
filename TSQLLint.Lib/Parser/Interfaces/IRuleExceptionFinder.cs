using System.Collections.Generic;
using System.IO;

namespace TSQLLint.Lib.Standard.Parser.Interfaces
{
    public interface IRuleExceptionFinder
    {
        IEnumerable<IRuleException> GetIgnoredRuleList(Stream fileStream);
    }
}
