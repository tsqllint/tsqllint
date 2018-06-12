using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Core;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Configuration.Overrides;
using TSQLLint.Infrastructure.Interfaces;

namespace TSQLLint.Infrastructure.Parser
{
    public class FragmentBuilder : IFragmentBuilder
    {
        private readonly TSqlParser parser;

        public FragmentBuilder() : this(Constants.DefaultCompatabilityLevel)
        {
        }

        public FragmentBuilder(int compatabilityLevel)
        {
            parser = GetSqlParser(CompatabilityLevel.Validate(compatabilityLevel));
        }

        public TSqlFragment GetFragment(TextReader txtRdr, out IList<ParseError> errors, IEnumerable<IOverride> overrides = null)
        {
            TSqlFragment fragment;

            OverrideCompatabilityLevel compatibilityLevel = null;
            if (overrides != null)
            {
                foreach (var lintingOverride in overrides)
                {
                    if (lintingOverride is OverrideCompatabilityLevel overrideCompatability)
                    {
                        compatibilityLevel = overrideCompatability;
                    }
                }
            }

            if (compatibilityLevel != null )
            {
                var tempParser = GetSqlParser(compatibilityLevel.CompatabilityLevel);
                fragment = tempParser.Parse(txtRdr, out errors);
                return fragment?.FirstTokenIndex != -1 ? fragment : null;
            }

            fragment = parser.Parse(txtRdr, out errors);
            return fragment?.FirstTokenIndex != -1 ? fragment : null;
        }

        private static TSqlParser GetSqlParser(int compatabilityLevel)
        {
            compatabilityLevel = CompatabilityLevel.Validate(compatabilityLevel);
            var fullyQualifiedName = string.Format("Microsoft.SqlServer.TransactSql.ScriptDom.TSql{0}Parser", compatabilityLevel);

            TSqlParser parser = null;

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var parserType = asm.GetType(fullyQualifiedName);
                if (parserType != null)
                {
                    parser = (TSqlParser)Activator.CreateInstance(parserType, new object[] { true });
                    break;
                }
            }

            return parser ?? new TSql120Parser(true);
        }
    }
}
