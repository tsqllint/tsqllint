using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TSQLLint.Common;

namespace TSQLLint.Infrastructure.Helpers
{
    public static class FixHelpers
    {
        public static (TReturn, TFind) FindViolatingNode<TFind, TReturn>(
             List<string> fileLines, IRuleViolation ruleViolation, Func<TFind, TReturn> getFragment)
             where TFind : TSqlFragment
             where TReturn : TSqlFragment
        {
            using var rdr = new StringReader(string.Join("\n", fileLines));
            var parser = new TSql150Parser(true, SqlEngineType.All);
            var tree = parser.Parse(rdr, out var errors);

            if (errors?.Any() == true)
            {
                throw new Exception($"Parsing failed. {string.Join(". ", errors.Select(x => x.Message))}");
            }

            var checker = new FindViolatingNodeVisitor<TFind>(ruleViolation);
            tree.Accept(checker);

            var node = checker.Nodes.FirstOrDefault(x =>
            {
                var fragment = getFragment(x);
                return fragment?.StartLine == ruleViolation.Line &&
                       fragment?.StartColumn == ruleViolation.Column;
            });

            return (getFragment(node), node);
        }

        public static T FindViolatingNode<T>(List<string> fileLines, IRuleViolation ruleViolation)
            where T : TSqlFragment
        {
            return FindViolatingNode<T, T>(fileLines, ruleViolation, x => x).Item1;
        }

        public static string GetIndent(List<string> fileLines, IRuleViolation ruleViolation)
        {
            return GetIndent(fileLines[ruleViolation.Line - 1]);
        }

        public static string GetIndent(List<string> fileLines, TSqlStatement statement)
        {
            return GetIndent(fileLines[statement.StartLine - 1]);
        }

        private static string GetIndent(string ifLine)
        {
            var ifPrefix = new Regex(@"^\s").Match(ifLine);

            var indent = string.Empty;
            if (ifPrefix.Success)
            {
                indent = ifPrefix.Value;
            }

            return indent;
        }

        public class FindViolatingNodeVisitor<T> : TSqlFragmentVisitor
            where T : TSqlFragment
        {
            public List<T> Nodes = new();
            private readonly IRuleViolation ruleViolation;

            public FindViolatingNodeVisitor(IRuleViolation ruleViolation)
            {
                this.ruleViolation = ruleViolation;
            }

            public override void Visit(TSqlFragment node)
            {
                if (node is T fragment)
                {
                    Nodes.Add(fragment);
                }

                base.Visit(node);
            }
        }
    }
}
