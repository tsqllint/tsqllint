using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Parser;

namespace TSQLLint.Infrastructure.Rules.RuleExceptions
{
    public class RuleExceptionFinder : IRuleExceptionFinder
    {
        public IEnumerable<IExtendedRuleException> GetIgnoredRuleList(Stream fileStream)
        {
            const string pattern = @"(tsqllint-(?:dis|en)able)\s*(.*)";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            var ruleExceptionList = new List<IExtendedRuleException>();
            TextReader reader = new StreamReader(fileStream);

            var lineNumber = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                var match = regex.Match(line);

                if (!match.Success)
                {
                    continue;
                }

                FindIgnoredRules(ruleExceptionList, lineNumber, match);
            }

            fileStream.Seek(0, SeekOrigin.Begin);

            foreach (var ruleException in ruleExceptionList)
            {
                if (ruleException.EndLine == 0)
                {
                    ruleException.SetEndLine(lineNumber);
                }
            }

            return ruleExceptionList;
        }

        private static void FindIgnoredRules(ICollection<IExtendedRuleException> ruleExceptionList, int lineNumber, Match match)
        {
            var action = match.Groups[1].Value;

            var disableCommand = action.Equals("tsqllint-disable", StringComparison.OrdinalIgnoreCase);
            var enableCommand = action.Equals("tsqllint-enable", StringComparison.OrdinalIgnoreCase);

            var ruleExceptionDetails = match.Groups[2].Value.Split(' ').Select(p => p.Trim()).ToList();
            var matchedFriendlyNames = ruleExceptionDetails.Intersect(RuleVisitorFriendlyNameTypeMap.List.Select(friendly => friendly.Key)).ToList();

            if (!matchedFriendlyNames.Any())
            {
                if (disableCommand)
                {
                    var ruleException = new GlobalRuleException(lineNumber, 0);
                    ruleExceptionList.Add(ruleException);
                }

                if (enableCommand)
                {
                    var ruleException = ruleExceptionList.OfType<GlobalRuleException>().FirstOrDefault(r => r.EndLine == 0);
                    ruleException?.SetEndLine(lineNumber);
                }
            }

            foreach (var matchedFriendlyName in matchedFriendlyNames)
            {
                RuleVisitorFriendlyNameTypeMap.List.TryGetValue(matchedFriendlyName, out var matchedType);

                if (disableCommand)
                {
                    var ruleException = new RuleException(matchedType, matchedFriendlyName, lineNumber, 0);
                    ruleExceptionList.Add(ruleException);
                }

                if (enableCommand)
                {
                    var ruleException = ruleExceptionList.OfType<RuleException>().FirstOrDefault(r => r.RuleName == matchedFriendlyName && r.EndLine == 0);
                    ruleException?.SetEndLine(lineNumber);
                }
            }
        }
    }
}
