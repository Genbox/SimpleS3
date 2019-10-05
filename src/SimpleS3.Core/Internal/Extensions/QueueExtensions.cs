using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Internal.Extensions
{
    /// <summary>This class is only necessary as long as we target .NET Standard 2.0</summary>
    internal static class QueueExtensions
    {
        public static bool TryDequeue<T>(this Queue<T> queue, out T value)
        {
            if (queue.Count > 0)
            {
                value = queue.Dequeue();
                return true;
            }

            value = default;
            return false;
        }
    }
}