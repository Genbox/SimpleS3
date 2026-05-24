using System.Runtime.ExceptionServices;

namespace Genbox.SimpleS3.Core.Common.Helpers;

public static class ParallelHelper
{
    public static async Task ExecuteAsync<T>(IEnumerable<T> source, Func<T, CancellationToken, Task> action, int concurrentThreads, CancellationToken token = default)
    {
        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
        using SemaphoreSlim throttler = new SemaphoreSlim(concurrentThreads);
        using IEnumerator<T> enumerator = source.GetEnumerator();
        List<Task> tasks = new List<Task>();
        CancellationToken innerToken = cancellation.Token;

        try
        {
            while (true)
            {
                await throttler.WaitAsync(innerToken).ConfigureAwait(false);

                if (!enumerator.MoveNext())
                {
                    throttler.Release();
                    break;
                }

                tasks.Add(ExecuteElementAsync(enumerator.Current));
            }
        }
        catch
        {
            await WaitForRunningTasksAsync(tasks).ConfigureAwait(false);
            ThrowFirstTaskFailure(tasks);
            throw;
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        async Task ExecuteElementAsync(T element)
        {
            try
            {
                await action(element, innerToken).ConfigureAwait(false);
            }
            catch
            {
                cancellation.Cancel();
                throw;
            }
            finally
            {
                throttler.Release();
            }
        }
    }

    public static async Task<IEnumerable<TReturn>> ExecuteAsync<T, TReturn>(IEnumerable<T> source, Func<T, CancellationToken, Task<TReturn>> action, int concurrentThreads, CancellationToken token = default)
    {
        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
        List<Task<TReturn>> tasks = new List<Task<TReturn>>();

        using SemaphoreSlim throttler = new SemaphoreSlim(concurrentThreads);
        using IEnumerator<T> enumerator = source.GetEnumerator();
        CancellationToken innerToken = cancellation.Token;

        try
        {
            while (true)
            {
                await throttler.WaitAsync(innerToken).ConfigureAwait(false);

                if (!enumerator.MoveNext())
                {
                    throttler.Release();
                    break;
                }

                tasks.Add(ExecuteElementAsync(enumerator.Current));
            }
        }
        catch
        {
            await WaitForRunningTasksAsync(tasks).ConfigureAwait(false);
            ThrowFirstTaskFailure(tasks);
            throw;
        }

        return await Task.WhenAll(tasks).ConfigureAwait(false);

        async Task<TReturn> ExecuteElementAsync(T element)
        {
            try
            {
                return await action(element, innerToken).ConfigureAwait(false);
            }
            catch
            {
                cancellation.Cancel();
                throw;
            }
            finally
            {
                throttler.Release();
            }
        }
    }

    private static async Task WaitForRunningTasksAsync(IEnumerable<Task> tasks)
    {
        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch
        {
            // Preserve the original producer/cancellation failure.
        }
    }

    private static void ThrowFirstTaskFailure(IEnumerable<Task> tasks)
    {
        foreach (Task task in tasks)
        {
            if (task.Exception?.InnerExceptions.Count > 0)
                ExceptionDispatchInfo.Capture(task.Exception.InnerExceptions[0]).Throw();
        }
    }
}