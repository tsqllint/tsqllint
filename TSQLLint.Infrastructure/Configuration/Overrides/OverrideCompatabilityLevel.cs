using TSQLLint.Core;
using TSQLLint.Core.Interfaces;

namespace TSQLLint.Infrastructure.Configuration.Overrides
{
    public class OverrideCompatabilityLevel : IOverride
    {
        public OverrideCompatabilityLevel(string value)
        {
            if (int.TryParse(value, out var parsedCompatabilityLevel))
            {
                CompatabilityLevel =
                    Core.CompatabilityLevel.Validate(parsedCompatabilityLevel);
            }
            else
            {
                CompatabilityLevel = Constants.DefaultCompatabilityLevel;
            }
        }

        public int CompatabilityLevel { get; }
    }
}
