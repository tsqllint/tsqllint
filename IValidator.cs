namespace TSQLLINT
{
    internal interface IValidator<T>
    {
        bool Validate(T t);
    }
}