using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLint.Infrastructure.Interfaces
{
    public interface IRuleVisitorBuilder
    {
        List<TSqlFragmentVisitor> BuildVisitors(string sqlPath, IEnumerable<IRuleException> ignoredRules);
    }
}
