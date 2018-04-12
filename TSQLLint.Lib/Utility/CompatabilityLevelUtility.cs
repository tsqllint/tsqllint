using System.Collections.Generic;

namespace TSQLLint.Lib.Utility
{
    public static class CompatabilityLevelUtility
    {
        public static int ValidateCompatabilityLevel(int compatabilityLevel)
        {
            var validCompatibilityLevels = new List<int> { 80, 90, 100, 110, 120, 130 };
            return validCompatibilityLevels.Contains(compatabilityLevel)
                ? compatabilityLevel
                : 120;
        }
    }
}
