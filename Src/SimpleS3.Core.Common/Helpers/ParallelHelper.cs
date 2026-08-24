using System.Runtime.ExceptionServices;

namespace Genbox.SimpleS3.Core.Common.Helpers;

/// <summary>Executes asynchronous operations with bounded concurrency and coordinated cancellation.</summary>
public static class ParallelHelper
{
    /// <summary>Executes an asynchronous action for every item while limiting the number of concurrent operations.</summary>
    /// <typeparam name="T">The source item type.</typeparam>
    /// <param name="source">The items to process.</param>
    /// <param name="action">The asynchronous action to invoke for each item.</param>
    /// <param name="concurrentThreads">The maximum number of actions that may run concurrently.</param>
    /// <param name="token">A token that cancels enumeration and all running actions.</param>
    public static async Task ExecuteAsync<T>(IEnumerable<T> source, Func<T, CancellationToken, Task> action, int concurrentThreads, CancellationToken token = default)
    {
        ValidateArguments(source, action, concurrentThreads);

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

                tasks.Add(ExecuteElementAsync(enumerator.Current, action, cancellation, innerToken, throttler));
            }
        }
        catch
        {
            cancellation.Cancel();
            await WaitForRunningTasksAsync(tasks).ConfigureAwait(false);
            ThrowFirstTaskFailure(tasks);
            throw;
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);

        static async Task ExecuteElementAsync(T element, Func<T, CancellationToken, Task> action, CancellationTokenSource cancellation, CancellationToken innerToken, SemaphoreSlim throttler)
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

    /// <summary>Executes an asynchronous projection for every item while limiting concurrency.</summary>
    /// <typeparam name="T">The source item type.</typeparam>
    /// <typeparam name="TReturn">The projected result type.</typeparam>
    /// <param name="source">The items to process.</param>
    /// <param name="action">The asynchronous projection to invoke for each item.</param>
    /// <param name="concurrentThreads">The maximum number of projections that may run concurrently.</param>
    /// <param name="token">A token that cancels enumeration and all running projections.</param>
    /// <returns>The projected results in source order.</returns>
    public static async Task<IEnumerable<TReturn>> ExecuteAsync<T, TReturn>(IEnumerable<T> source, Func<T, CancellationToken, Task<TReturn>> action, int concurrentThreads, CancellationToken token = default)
    {
        ValidateArguments(source, action, concurrentThreads);

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

                tasks.Add(ExecuteElementAsync(enumerator.Current, action, cancellation, innerToken, throttler));
            }
        }
        catch
        {
            cancellation.Cancel();
            await WaitForRunningTasksAsync(tasks).ConfigureAwait(false);
            ThrowFirstTaskFailure(tasks);
            throw;
        }

        return await Task.WhenAll(tasks).ConfigureAwait(false);

        static async Task<TReturn> ExecuteElementAsync(T element, Func<T, CancellationToken, Task<TReturn>> action, CancellationTokenSource cancellation, CancellationToken innerToken, SemaphoreSlim throttler)
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

    /// <summary>Asynchronously enumerates and projects every item while limiting concurrency.</summary>
    /// <typeparam name="T">The source item type.</typeparam>
    /// <typeparam name="TReturn">The projected result type.</typeparam>
    /// <param name="source">The asynchronous sequence to process.</param>
    /// <param name="action">The asynchronous projection to invoke for each item.</param>
    /// <param name="concurrentThreads">The maximum number of projections that may run concurrently.</param>
    /// <param name="token">A token that cancels enumeration and all running projections.</param>
    /// <returns>The projected results in source order.</returns>
    public static async Task<IEnumerable<TReturn>> ExecuteAsync<T, TReturn>(IAsyncEnumerable<T> source, Func<T, CancellationToken, Task<TReturn>> action, int concurrentThreads, CancellationToken token = default)
    {
        ValidateArguments(source, action, concurrentThreads);

        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
        List<Task<TReturn>> tasks = new List<Task<TReturn>>();

        using SemaphoreSlim throttler = new SemaphoreSlim(concurrentThreads);
        IAsyncEnumerator<T> enumerator = source.GetAsyncEnumerator(cancellation.Token);
        CancellationToken innerToken = cancellation.Token;

        try
        {
            try
            {
                while (true)
                {
                    await throttler.WaitAsync(innerToken).ConfigureAwait(false);

                    if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
                    {
                        throttler.Release();
                        break;
                    }

                    tasks.Add(ExecuteElementAsync(enumerator.Current, action, cancellation, innerToken, throttler));
                }
            }
            catch
            {
                cancellation.Cancel();
                await WaitForRunningTasksAsync(tasks).ConfigureAwait(false);
                ThrowFirstTaskFailure(tasks);
                throw;
            }

            return await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        finally
        {
            await enumerator.DisposeAsync().ConfigureAwait(false);
        }

        static async Task<TReturn> ExecuteElementAsync(T element, Func<T, CancellationToken, Task<TReturn>> action, CancellationTokenSource cancellation, CancellationToken innerToken, SemaphoreSlim throttler)
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

    private static void ValidateArguments(object? source, Delegate? action, int concurrentThreads)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (concurrentThreads <= 0)
            throw new ArgumentOutOfRangeException(nameof(concurrentThreads), concurrentThreads, "Concurrency must be greater than zero.");
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