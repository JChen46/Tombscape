namespace StdbModule.Util
{
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
}