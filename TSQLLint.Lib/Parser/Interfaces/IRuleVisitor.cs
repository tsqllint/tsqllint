using System.IO;

namespace TSQLLint.Lib.Parser.Interfaces
{
    public interface IRuleVisitor
    {
        void VisitRules(string path, Stream sqlFileStream);
    }
}
