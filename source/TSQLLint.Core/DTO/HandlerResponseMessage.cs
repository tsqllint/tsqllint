namespace TSQLLint.Core.DTO
{
    public class HandlerResponseMessage
    {
        public HandlerResponseMessage(bool success, bool shouldLint)
            : this(success, shouldLint, false)
        {
        }

        public HandlerResponseMessage(bool success, bool shouldLint, bool shouldFix)
        {
            Success = success;
            ShouldLint = shouldLint;
            ShouldFix = shouldFix;
        }

        public bool Success { get; }
        public bool ShouldLint { get; }
        public bool ShouldFix { get; }
    }
}
