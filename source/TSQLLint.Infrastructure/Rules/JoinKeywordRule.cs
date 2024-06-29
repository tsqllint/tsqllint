using Microsoft.SqlServer.TransactSql.ScriptDom;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TSQLLint.Common;
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
            // Check if the join is using commas (implicit join syntax)
            if (node.TableReferences.Count > 1)
            {
                for (int i = 0; i < node.TableReferences.Count; i++)
                {
                    if (node.TableReferences[i] is QualifiedJoin)
                    {
                        // Skip if it's a proper JOIN
                        continue;
                    }
                    if (i < node.TableReferences.Count - 1)
                    {
                        // If the next table reference is not a JOIN, it's a comma join
                        if (!(node.TableReferences[i + 1] is QualifiedJoin))
                        {
                            errorCallback(RULE_NAME, RULE_TEXT, GetLineNumber(node), GetColumnNumber(node));
                            break;
                        }
                    }
                }
            }
            
            base.Visit(node);
        }

    }
}
