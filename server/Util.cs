namespace StdbModule;

public static class Util
{
    public static bool IsNull<T>(T? input, out T output) where T : struct
    {
        if (input is null)
        {
            output = default;
            return true;
        }

        output = input.Value;
        return false;
    }
    
    public static bool IsNotNull<T>(T? input, out T output) where T : struct
    {
        return !IsNull(input, out output);
    }
}