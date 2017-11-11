using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TSQLLint.Lib.Parser.Interfaces;

namespace TSQLLint.Lib.Parser
{
    public class RuleExceptionFinder
    {
        public IEnumerable<IRuleException> GetIgnoredRuleList(Stream fileStream)
        {
            const string pattern = @"\/\*\s*(tsqllint-(?:dis|en)able)\s*(.*)(?:\s*\*\/)";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            
            var ruleExceptionList = new List<IRuleException>();
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

            foreach (var ruleException in ruleExceptionList)
            {
                if (ruleException.EndLine == 0)
                {
                    ruleException.SetEndLine(lineNumber);
                }
            }
            
            return ruleExceptionList;
        }

        private static void FindIgnoredRules(ICollection<IRuleException> ruleExceptionList, int lineNumber, Match match)
        {
            var action = match.Groups[1].Value;
            var ruleExceptionDetails = match.Groups[2].Value.Split(' ').Select(p => p.Trim()).ToList();
            var matchedFriendlyNames = ruleExceptionDetails.Intersect(RuleVisitorFriendlyNameTypeMap.List.Select(friendly => friendly.Key)).ToList();

            if (!matchedFriendlyNames.Any())
            {
                if (action == "tsqllint-disable")
                {
                    var ruleException = new GlobalRuleException(lineNumber, 0);
                    ruleExceptionList.Add(ruleException);
                }

                if (action == "tsqllint-enable")
                {
                    var ruleException = ruleExceptionList.OfType<GlobalRuleException>().FirstOrDefault(r => r.EndLine == 0);
                    ruleException?.SetEndLine(lineNumber);
                }
            }

            foreach (var matchedFriendlyName in matchedFriendlyNames)
            {
                RuleVisitorFriendlyNameTypeMap.List.TryGetValue(matchedFriendlyName, out var matchedType);

                if (action == "tsqllint-disable")
                {
                    var ruleException = new RuleException(matchedType, lineNumber, 0);
                    ruleExceptionList.Add(ruleException);
                }

                if (action == "tsqllint-enable")
                {
                    var ruleException = ruleExceptionList.OfType<RuleException>().FirstOrDefault(r => r.EndLine == 0);
                    ruleException?.SetEndLine(lineNumber);
                }
            }
        }
    }
}
