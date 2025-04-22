namespace StdbModule.Util;

// Seems pretty useless here
public class Optional<T> where T : struct
{
    private T? Value { get; init; }

    public T? Get()
    {
        if (Value == null)
        {
            throw new NullReferenceException();
        }
        return Value;
    }

    public static Optional<T> Of(T value)
    {
        return new Optional<T>{Value = value};
    }

    public static Optional<T> Empty()
    {
        return new Optional<T>();
    }
}