using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLint.Infrastructure.Interfaces
{
    public interface IFragmentBuilder
    {
        TSqlFragment GetFragment(TextReader txtRdr, out IList<ParseError> errors);
    }
}
