using System.Reflection;

namespace TSQLLint.Core.Interfaces
{
    public interface IFileversionWrapper
    {
        string GetVersion(Assembly assembly);
    }
}
