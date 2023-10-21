using System.Collections.Generic;
using System.Linq;

namespace MagicBytesValidator.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<(T, int)> AsIndexed<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Select((v, i) => (v, i));
    }
}