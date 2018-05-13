using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace TSQLLint.Tests.Helpers
{
    public class EverythingVisitor : TSqlFragmentVisitor
    {
        public override void Visit(TSqlScript node)
        {
        }
    }
}
