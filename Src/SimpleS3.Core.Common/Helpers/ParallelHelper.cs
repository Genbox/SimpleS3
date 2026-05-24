namespace Genbox.SimpleS3.Core.Common.Helpers;

public static class ParallelHelper
{
    public static async Task ExecuteAsync<T>(IEnumerable<T> source, Func<T, CancellationToken, Task> action, int concurrentThreads, CancellationToken token = default)
    {
        using SemaphoreSlim throttler = new SemaphoreSlim(concurrentThreads);
        IEnumerable<Task> tasks = source.Select(ExecuteElementAsync);

        await Task.WhenAll(tasks).ConfigureAwait(false);

        async Task ExecuteElementAsync(T element)
        {
            bool acquired = false;

            try
            {
                token.ThrowIfCancellationRequested();
                await throttler.WaitAsync(token).ConfigureAwait(false);
                acquired = true;
                await action(element, token).ConfigureAwait(false);
            }
            finally
            {
                if (acquired)
                    throttler.Release();
            }
        }
    }

    public static async Task<IEnumerable<TReturn>> ExecuteAsync<T, TReturn>(IEnumerable<T> source, Func<T, CancellationToken, Task<TReturn>> action, int concurrentThreads, CancellationToken token = default)
    {
        List<Task<TReturn>> tasks = new List<Task<TReturn>>();

        using SemaphoreSlim throttler = new SemaphoreSlim(concurrentThreads);

        foreach (T t in source)
            tasks.Add(ExecuteElementAsync(t));

        return await Task.WhenAll(tasks).ConfigureAwait(false);

        async Task<TReturn> ExecuteElementAsync(T element)
        {
            bool acquired = false;

            try
            {
                await throttler.WaitAsync(token).ConfigureAwait(false);
                acquired = true;
                return await action(element, token).ConfigureAwait(false);
            }
            finally
            {
                if (acquired)
                    throttler.Release();
            }
        }
    }
}