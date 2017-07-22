namespace TSQLLINT_CONSOLE.CommandLineParser
{
    internal interface IValidator<T>
    {
        bool Validate(T t);
    }
}