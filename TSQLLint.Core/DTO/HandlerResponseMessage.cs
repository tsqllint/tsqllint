namespace TSQLLint.Core.DTO
{
    public class HandlerResponseMessage
    {
        public HandlerResponseMessage(bool success, bool shouldLint)
        {
            Success = success;
            ShouldLint = shouldLint;
        }
        public bool Success { get; }
        
        public bool ShouldLint { get; }
    }
}
