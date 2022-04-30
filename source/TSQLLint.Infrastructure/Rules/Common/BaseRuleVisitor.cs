using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.IO;
using System.Linq;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Rules.Common
{
    public abstract class BaseRuleVisitor : TSqlFragmentVisitor, ISqlRule
    {
        protected readonly Action<string, string, int, int> errorCallback;

        public BaseRuleVisitor(Action<string, string, int, int> errorCallback)
        {
            this.errorCallback = errorCallback;
        }

        public int DynamicSqlStartColumn { get; set; }
        public int DynamicSqlStartLine { get; set; }
        public abstract string RULE_NAME { get; }
        public abstract string RULE_TEXT { get; }

        public virtual T FindViolatingNode<T>(string[] fileLines, IRuleViolation ruleViolation)
            where T : TSqlFragment
        {
            using var rdr = new StringReader(string.Join("\n", fileLines));
            var parser = new TSql150Parser(true, SqlEngineType.All);
            var tree = parser.Parse(rdr, out var errors);
            if (errors?.Any() == true)
            {
                throw new Exception($"Parsing failed. {string.Join(". ", errors.Select(x => x.Message))}");
            }
            var checker = new FineViolatingNodeVisitor<T>(ruleViolation);
            tree.Accept(checker);
            return checker.Node;
        }

        public virtual void FixViolation(string[] fileLines, IRuleViolation ruleViolation)
        {
        }

        internal class FineViolatingNodeVisitor<T> : TSqlFragmentVisitor
            where T : TSqlFragment
        {
            public T Node;
            private readonly IRuleViolation ruleViolation;

            public FineViolatingNodeVisitor(IRuleViolation ruleViolation)
            {
                this.ruleViolation = ruleViolation;
            }

            public override void Visit(TSqlFragment node)
            {
                if (node is T fragment &&
                    fragment.StartLine == ruleViolation.Line &&
                    fragment.StartColumn == ruleViolation.Column)
                {
                    Node = fragment;

                    return;
                }

                base.Visit(node);
            }
        }
    }
}
