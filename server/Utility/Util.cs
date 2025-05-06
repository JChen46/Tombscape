using SpacetimeDB;

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

    public const string StringChars = "0123456789abcdef";

    public static string GenerateUniqueHexString(int length)
    {
        Random rand = new Random();
        var charList = StringChars.ToArray();
        string hexString = "";
      
        for(int i = 0; i < length; i++)
        {
            int randIndex = rand.Next(0, charList.Length);
            hexString += charList[randIndex];
        }

        return hexString;
    }

    public static string LogDbVector2(DbVector2 vector)
    {
        return $"x: {vector.x}, y: {vector.y}";
    }

    // public static void DebugLogTableData( ITableView tableCtx, string tableName = "") 
    // {
    //     Log.Debug($"Table {tableName} count: {tableCtx.Count}");
    // }
    
    
}

public static class EnumerableExtensions
{
    public static T? TryGetFirst<T>(this IEnumerable<T> source) where T : struct
    {
        using var enumerator = source.GetEnumerator();
        if (enumerator.MoveNext())
        {
            return enumerator.Current;
        }
        else
        {
            return null;
        }
    }
}