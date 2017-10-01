using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Lib.Rules.RuleViolations;

namespace TSQLLint.Lib.Parser.Interfaces
{
    public interface IRuleVisitor
    {
        List<RuleViolation> Violations { get; set; }

        void VisitRule(TextReader txtRdr, TSqlFragmentVisitor visitor);

        void VisitRules(string path, TextReader txtRdr);
    }
}