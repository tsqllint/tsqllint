using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSQLLint.Core.Interfaces
{
    public interface IGlobPatternMatcher
    {
        IEnumerable<string> GetResultsInFullPath(string path);
    }
}
