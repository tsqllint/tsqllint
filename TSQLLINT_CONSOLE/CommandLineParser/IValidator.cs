namespace TSQLLINT_CONSOLE
{
    internal interface IValidator<T>
    {
        bool Validate(T t);
    }
}