using TSQLLint.Common;

namespace TSQLLint.Infrastructure.Interfaces
{
    public interface IViolationFixer
    {
        void AddViolation(IRuleViolation violation);
        void FixViolations();
    }
}
