using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TSQLLint.Lib.Parser.ConfigurationOverrides
{
    public class OverrideFinder
    {
        public IEnumerable<IOverride> GetOverrideList(Stream fileStream)
        {
            const string pattern = @"(\/\*) ?tsqllint-override: ?(.* += +.*)+(\*\/)";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            var overrideList = new List<IOverride>();
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

                var overrideDetails = match.Groups[2].Value.Split(',').Select(p => p.Trim()).ToList();
                foreach (var overrideDetail in overrideDetails)
                {
                    var details = overrideDetail.Split(' ').Select(p => p.Trim()).ToList();
                    if (OverrideTypeMap.List.ContainsKey(details[0]))
                    {
                        var overrideTypeExists = OverrideTypeMap.List.TryGetValue(details[0], out var overrideType);
                        if (overrideTypeExists)
                        {
                            overrideList.Add((IOverride)Activator.CreateInstance(overrideType, details[2]));
                        }
                    }
                }
            }

            fileStream.Seek(0, SeekOrigin.Begin);

            return overrideList;
        }
    }
}
