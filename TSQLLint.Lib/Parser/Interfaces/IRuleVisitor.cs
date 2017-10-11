using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLint.Lib.Parser.Interfaces
{
    public interface IRuleVisitor
    {
        void VisitRule(TextReader txtRdr, TSqlFragmentVisitor visitor);

        void VisitRules(string path, TextReader txtRdr);
    }
}
