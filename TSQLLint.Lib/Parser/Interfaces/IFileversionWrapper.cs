namespace TSQLLint.Lib.Plugins
{
    public interface IFileversionWrapper
    {
        string GetVersion(System.Reflection.Assembly assembly);
    }
}