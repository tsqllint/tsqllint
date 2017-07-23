namespace TSQLLINT_CONSOLE.CommandLineOptions.Interfaces
{
    internal interface IValidator<T>
    {
        bool Validate(T t);
    }
}