using System.Collections.Generic;
using System.IO;
using TSQLLint.Common;

namespace TSQLLint.Infrastructure.Plugins
{
    public class PluginContext : IPluginContext
    {
        public PluginContext(string filePath, IEnumerable<IRuleException> ruleExceptions, TextReader fileContents)
        {
            FilePath = filePath;
            FileContents = fileContents;
            RuleExceptions = ruleExceptions;
        }

        public string FilePath { get; }

        public TextReader FileContents { get; }

        public IEnumerable<IRuleException> RuleExceptions { get; }
    }
}
