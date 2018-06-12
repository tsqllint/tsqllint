using System.Collections.Generic;
using System.IO;

namespace TSQLLint.Core.Interfaces
{
    public interface IRuleExceptionFinder
    {
        IEnumerable<IExtendedRuleException> GetIgnoredRuleList(Stream fileStream);
    }
}
