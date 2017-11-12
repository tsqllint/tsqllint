using System.IO;

namespace TSQLLint.Lib.Standard.Parser.Interfaces
{
    public interface IRuleVisitor
    {
        void VisitRules(string path, Stream sqlFileStream);
    }
}
