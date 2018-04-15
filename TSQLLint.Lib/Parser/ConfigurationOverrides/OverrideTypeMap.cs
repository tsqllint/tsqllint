using System;
using System.Collections.Generic;

namespace TSQLLint.Lib.Parser.ConfigurationOverrides
{
    public class OverrideTypeMap
    {
        public static readonly Dictionary<string, Type> List = new Dictionary<string, Type>
        {
            { "compatability-level", typeof(OverrideCompatabilityLevel) }
        };
    }
}
