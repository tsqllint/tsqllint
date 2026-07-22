using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Rules.Common;

namespace TSQLLint.Infrastructure.Rules
{
    public class JoinKeywordRule : BaseRuleVisitor, ISqlRule
    {
        public JoinKeywordRule(System.Action<string, string, int, int> errorCallback)
            : base(errorCallback)
        {
        }

        public override string RULE_NAME => "join-keyword";

        public override string RULE_TEXT => "Join keyword should be used rather than implicit join syntax (comma joins).  Replace comma joins with 'INNER JOIN' syntax";

        public override void Visit(FromClause node)
        {
            // More than one top-level table reference in a FROM clause means the tables
            // are separated by commas (implicit join syntax). Explicit JOIN, APPLY, PIVOT,
            // derived tables and table-valued functions all nest into a single table
            // reference, so any count greater than one indicates a comma join -- including
            // mixed forms such as "FROM A, B INNER JOIN C" that the previous per-element
            // check let slip through. (#332)
            if (node.TableReferences.Count > 1)
            {
                errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(node), GetColumnNumber(node));
            }

            base.Visit(node);
        }
    }
}
