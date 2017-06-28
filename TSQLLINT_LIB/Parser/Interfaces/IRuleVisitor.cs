using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLINT_LIB.Config;
using TSQLLINT_LIB.Rules.RuleViolations;

namespace TSQLLINT_LIB.Parser.Interfaces
{
    public interface IRuleVisitor
    {
        List<RuleViolation> Violations { get; set; }
        void VisistRule(TextReader txtRdr, TSqlFragmentVisitor visitor);
        void VisitRules(ILintConfigReader configReader, string path, TextReader txtRdr);
    }
}