using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Parser.RuleExceptions;

namespace TSQLLint.Lib.Parser.Interfaces
{
    public interface IRuleVisitorBuilder
    {
        List<TSqlFragmentVisitor> BuildVisitors(string sqlPath, List<IExtendedRuleException> ignoredRules);
    }
}
