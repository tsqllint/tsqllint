using System.Collections.Generic;
using System.IO;
using TSQLLint.Common;

namespace TSQLLint.Core.Interfaces
{
    public interface IRuleVisitor
    {
        void VisitRules(string path, IEnumerable<IRuleException> igoredRules, Stream sqlFileStream);
    }
}
