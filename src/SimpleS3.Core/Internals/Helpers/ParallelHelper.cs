using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Genbox.SimpleS3.Core.Internals.Helpers
{
    public static class ParallelHelper
    {
        public static async Task ExecuteAsync<T>(IEnumerable<T> source, Func<T, Task> action, int concurrentThreads, CancellationToken token = default)
        {
            using (SemaphoreSlim throttler = new SemaphoreSlim(concurrentThreads))
            {
                IEnumerable<Task> tasks = source.Select(async element =>
                {
                    try
                    {
                        await throttler.WaitAsync(token).ConfigureAwait(false);
                        await action(element).ConfigureAwait(false);
                    }
                    finally
                    {
                        throttler.Release();
                    }
                });

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }

        public static async Task<IEnumerable<TReturn>> ExecuteAsync<T, TReturn>(IEnumerable<T> source, Func<T, Task<TReturn>> action, int concurrentThreads, CancellationToken token = default)
        {
            List<Task<TReturn>> tasks = new List<Task<TReturn>>();
            List<Task> tasks2 = new List<Task>();

            using (SemaphoreSlim throttler = new SemaphoreSlim(concurrentThreads))
            {
                foreach (T t in source)
                {
                    await throttler.WaitAsync(token);

                    Task<TReturn>? b = action(t);
                    tasks.Add(b);
                    tasks2.Add(b.ContinueWith(x => throttler.Release(), token, TaskContinuationOptions.None, TaskScheduler.Current));
                }

                await Task.WhenAll(tasks2);
                return await Task.WhenAll(tasks);
            }
        }
    }
}