using System;
using System.Collections.Generic;

namespace TSQLLint.Infrastructure.Configuration.Overrides
{
    public class OverrideTypeMap
    {
        public static readonly Dictionary<string, Type> List = new Dictionary<string, Type>
        {
            { "compatability-level", typeof(OverrideCompatabilityLevel) }
        };
    }
}
