using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TSQLLint.Core;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Configuration.Overrides
{
    public class OverrideFinder
    {
        private static Regex OverrideRegex = new Regex(@".*?tsqllint-override ?(.* += +.*)+.*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public IEnumerable<IOverride> GetOverrideList(Stream fileStream)
        {
            var overrideList = new List<IOverride>();
            TextReader reader = new StreamReader(fileStream);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length > Constants.MaxLineWidthForRegexEval || ! line.Contains("tsqllint-override"))
                {
                    continue;
                }
                
                var match = OverrideRegex.Match(line);
                if (!match.Success)
                {
                    continue;
                }

                var overrideDetails = match.Groups[1].Value.Split(',').Select(p => p.Trim()).ToList();
                foreach (var overrideDetail in overrideDetails)
                {
                    var details = overrideDetail.Split(' ').Select(p => p.Trim()).ToList();
                    if (OverrideTypeMap.List.ContainsKey(details[0]))
                    {
                        var overrideType = OverrideTypeMap.List.GetValueOrDefault(details[0]);
                        overrideList.Add((IOverride)Activator.CreateInstance(overrideType, details[2]));
                    }
                }
            }

            fileStream.Seek(0, SeekOrigin.Begin);

            return overrideList;
        }
    }
}
