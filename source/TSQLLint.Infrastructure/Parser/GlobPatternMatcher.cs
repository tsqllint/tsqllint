using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Parser
{
    public class GlobPatternMatcher : IGlobPatternMatcher
    {
        private readonly Matcher matcher = new Matcher();

        public GlobPatternMatcher(Matcher matcher)
        {
            this.matcher = matcher;
        }

        public IEnumerable<string> GetResultsInFullPath(string path)
        {
            return matcher.GetResultsInFullPath(path);
        }
    }
}
