using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Interfaces
{
    public interface IFragmentBuilder
    {
        TSqlFragment GetFragment(TextReader txtRdr, out IList<ParseError> errors, IEnumerable<IOverride> overrides = null);
    }
}
