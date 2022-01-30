namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class ParallelHelper
{
    public static async Task ExecuteAsync<T>(IEnumerable<T> source, Func<T, CancellationToken, Task> action, int concurrentThreads, CancellationToken token = default)
    {
        using (SemaphoreSlim throttler = new SemaphoreSlim(concurrentThreads))
        {
            IEnumerable<Task> tasks = source.Select(async element =>
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    await throttler.WaitAsync(token).ConfigureAwait(false);
                    await action(element, token).ConfigureAwait(false);
                }
                finally
                {
                    throttler.Release();
                }
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }

    public static async Task<IEnumerable<TReturn>> ExecuteAsync<T, TReturn>(IEnumerable<T> source, Func<T, CancellationToken, Task<TReturn>> action, int concurrentThreads, CancellationToken token = default)
    {
        List<Task<TReturn>> tasks = new List<Task<TReturn>>();
        List<Task> tasks2 = new List<Task>();

        using (SemaphoreSlim throttler = new SemaphoreSlim(concurrentThreads))
        {
            foreach (T t in source)
            {
                await throttler.WaitAsync(token);

                Task<TReturn>? b = action(t, token);
                tasks.Add(b);
                tasks2.Add(b.ContinueWith(x => throttler.Release(), token, TaskContinuationOptions.None, TaskScheduler.Current));
            }

            await Task.WhenAll(tasks2);
            return await Task.WhenAll(tasks);
        }
    }
}