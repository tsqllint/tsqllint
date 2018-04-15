namespace TSQLLint.Lib.Parser.ConfigurationOverrides
{
    public class OverrideCompatabilityLevel : IOverride
    {
        public OverrideCompatabilityLevel(string value)
        {
            if (int.TryParse(value, out var parsedCompatabilityLevel))
            {
                CompatabilityLevel = Utility.CompatabilityLevelUtility.ValidateCompatabilityLevel(parsedCompatabilityLevel);
            }
        }

        public int CompatabilityLevel { get; }
    }
}
