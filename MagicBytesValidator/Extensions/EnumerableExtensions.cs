namespace MagicBytesValidator.Extensions;

[Obsolete("Not used by MagicBytesValidator anymore. Use Enumerable.Select((value, index) => (value, index)) instead. EnumerableExtensions will be removed in version 3.0.")]
public static class EnumerableExtensions
{
    public static IEnumerable<(T, int)> AsIndexed<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Select((v, i) => (v, i));
    }
}