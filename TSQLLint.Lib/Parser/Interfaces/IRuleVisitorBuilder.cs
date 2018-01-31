using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;
using TSQLLint.Lib.Parser.RuleExceptions;

namespace TSQLLint.Lib.Parser.Interfaces
{
    public interface IRuleVisitorBuilder
    {
        List<TSqlFragmentVisitor> BuildVisitors(string sqlPath, IEnumerable<IRuleException> ignoredRules);
    }
}
