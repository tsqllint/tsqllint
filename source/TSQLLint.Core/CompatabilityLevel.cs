using System.Collections.Generic;

namespace TSQLLint.Core
{
    public static class CompatabilityLevel
    {
        public static int Validate(int compatabilityLevel)
        {
            var validCompatibilityLevels = new List<int> { 80, 90, 100, 110, 120, 130, 140, 150 };
            return validCompatibilityLevels.Contains(compatabilityLevel)
                ? compatabilityLevel
                : Constants.DefaultCompatabilityLevel;
        }
    }
}
