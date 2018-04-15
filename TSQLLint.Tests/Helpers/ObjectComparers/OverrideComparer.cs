using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TSQLLint.Lib.Parser.ConfigurationOverrides;

namespace TSQLLint.Tests.UnitTests.ConfigFile.Overrides
{
    [ExcludeFromCodeCoverage]
    public class OverrideComparer : IComparer, IComparer<IOverride>
    {
        public int Compare(object x, object y)
        {
            if (!(x is IOverride lhs) || !(y is IOverride rhs))
            {
                throw new InvalidOperationException("cannot compare null object");
            }

            return Compare(lhs, rhs);
        }

        public int Compare(IOverride x, IOverride y)
        {
            if (x.GetType() == typeof(OverrideCompatabilityLevel) && y.GetType() == typeof(OverrideCompatabilityLevel))
            {
                if (x is OverrideCompatabilityLevel lhs && y is OverrideCompatabilityLevel rhs && lhs.CompatabilityLevel != rhs.CompatabilityLevel)
                {
                    return -1;
                }
            }

            return 0;
        }
    }
}
