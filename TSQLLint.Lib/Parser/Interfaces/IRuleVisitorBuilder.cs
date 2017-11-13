using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLint.Lib.Parser.Interfaces
{
    public interface IRuleVisitorBuilder
    {
        List<TSqlFragmentVisitor> BuildVisitors(string sqlPath, List<IRuleException> ignoredRules);
    }
}
