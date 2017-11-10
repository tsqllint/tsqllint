using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLint.Lib.Parser.Interfaces
{
    public interface IRuleVisitor
    {
        void VisitRule(Stream fileStream, TSqlFragmentVisitor visitor);

        void VisitRules(string path, Stream sqlFileStream);
    }
}
