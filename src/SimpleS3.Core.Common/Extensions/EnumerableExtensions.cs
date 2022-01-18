using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Common.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<List<T>> Chunk<T>(this IEnumerable<T> enumerable, int chunkSize)
    {
        List<T> chunk = new List<T>(chunkSize);

        foreach (T item in enumerable)
        {
            chunk.Add(item);

            if (chunk.Count == chunkSize)
            {
                yield return chunk;
                chunk = new List<T>(chunkSize);
            }
        }

        //Last chunk
        if (chunk.Count > 0)
            yield return chunk;
    }
}