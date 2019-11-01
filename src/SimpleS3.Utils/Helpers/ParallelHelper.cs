using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Genbox.SimpleS3.Utils.Helpers
{
    public static class ParallelHelper
    {
        public static async Task<T[]> PerformInParallel<T>(IEnumerable<Task<T>> tasks, int concurrent = 10)
        {
            Queue<Task<T>> queue = new Queue<Task<T>>();

            using (SemaphoreSlim semaphore = new SemaphoreSlim(concurrent))
            {
                foreach (Task<T> task in tasks)
                {
                    await semaphore.WaitAsync().ConfigureAwait(false);

                    queue.Enqueue(PerformTask<T>(task, semaphore));
                }

                await Task.WhenAll(queue).ConfigureAwait(false);
            }

            return queue.Select(x => x.Result).ToArray();
        }

        public static async Task PerformInParallel(IEnumerable<Task> tasks, int concurrent = 10)
        {
            Queue<Task> queue = new Queue<Task>();

            using (SemaphoreSlim semaphore = new SemaphoreSlim(concurrent))
            {
                foreach (Task task in tasks)
                {
                    await semaphore.WaitAsync().ConfigureAwait(false);

                    queue.Enqueue(PerformTask(task, semaphore));
                }

                await Task.WhenAll(queue).ConfigureAwait(false);
            }
        }

        private static async Task PerformTask(Task task, SemaphoreSlim semaphore)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static async Task<T> PerformTask<T>(Task<T> task, SemaphoreSlim semaphore)
        {
            try
            {
                return await task.ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}