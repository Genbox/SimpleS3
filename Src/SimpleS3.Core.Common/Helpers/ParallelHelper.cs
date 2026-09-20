using System.Diagnostics.CodeAnalysis;
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

        CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
        SemaphoreSlim throttler = new SemaphoreSlim(concurrentThreads);

        try
        {
            using IEnumerator<T> enumerator = source.GetEnumerator();
            List<Task> tasks = new List<Task>();
            CancellationToken innerToken = cancellation.Token;
            Action cancel = cancellation.Cancel;
            Func<int> release = throttler.Release;

            try
            {
                while (true)
                {
                    await throttler.WaitAsync(innerToken);

                    if (!enumerator.MoveNext())
                    {
                        throttler.Release();
                        break;
                    }

                    tasks.Add(ExecuteElementAsync(enumerator.Current, action, cancel, innerToken, release));
                }
            }
            catch
            {
                cancellation.Cancel();
                await WaitForRunningTasksAsync(tasks);
                ThrowFirstTaskFailure(tasks);
                throw;
            }

            await Task.WhenAll(tasks);
        }
        finally
        {
            throttler.Dispose();
            cancellation.Dispose();
        }

        static async Task ExecuteElementAsync(T element, Func<T, CancellationToken, Task> action, Action cancel, CancellationToken innerToken, Func<int> release)
        {
            try
            {
                await action(element, innerToken);
            }
            catch
            {
                cancel();
                throw;
            }
            finally
            {
                release();
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

        CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
        List<Task<TReturn>> tasks = new List<Task<TReturn>>();

        SemaphoreSlim throttler = new SemaphoreSlim(concurrentThreads);

        try
        {
            using IEnumerator<T> enumerator = source.GetEnumerator();
            CancellationToken innerToken = cancellation.Token;
            Action cancel = cancellation.Cancel;
            Func<int> release = throttler.Release;

            try
            {
                while (true)
                {
                    await throttler.WaitAsync(innerToken);

                    if (!enumerator.MoveNext())
                    {
                        throttler.Release();
                        break;
                    }

                    tasks.Add(ExecuteElementAsync(enumerator.Current, action, cancel, innerToken, release));
                }
            }
            catch
            {
                cancellation.Cancel();
                await WaitForRunningTasksAsync(tasks);
                ThrowFirstTaskFailure(tasks);
                throw;
            }

            return await Task.WhenAll(tasks);
        }
        finally
        {
            throttler.Dispose();
            cancellation.Dispose();
        }

        static async Task<TReturn> ExecuteElementAsync(T element, Func<T, CancellationToken, Task<TReturn>> action, Action cancel, CancellationToken innerToken, Func<int> release)
        {
            try
            {
                return await action(element, innerToken);
            }
            catch
            {
                cancel();
                throw;
            }
            finally
            {
                release();
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
    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP016:Don't use disposed instance", Justification = "All workers complete before the async enumerator is disposed in the finally block.")]
    public static async Task<IEnumerable<TReturn>> ExecuteAsync<T, TReturn>(IAsyncEnumerable<T> source, Func<T, CancellationToken, Task<TReturn>> action, int concurrentThreads, CancellationToken token = default)
    {
        ValidateArguments(source, action, concurrentThreads);

        CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
        List<Task<TReturn>> tasks = new List<Task<TReturn>>();

        SemaphoreSlim throttler = new SemaphoreSlim(concurrentThreads);

        try
        {
            IAsyncEnumerator<T> enumerator = source.GetAsyncEnumerator(cancellation.Token);
            CancellationToken innerToken = cancellation.Token;
            Action cancel = cancellation.Cancel;
            Func<int> release = throttler.Release;

            try
            {
                try
                {
                    while (true)
                    {
                        await throttler.WaitAsync(innerToken);

                        if (!await enumerator.MoveNextAsync())
                        {
                            throttler.Release();
                            break;
                        }

                        tasks.Add(ExecuteElementAsync(enumerator.Current, action, cancel, innerToken, release));
                    }
                }
                catch
                {
                    cancellation.Cancel();
                    await WaitForRunningTasksAsync(tasks);
                    ThrowFirstTaskFailure(tasks);
                    throw;
                }

                return await Task.WhenAll(tasks);
            }
            finally
            {
                await enumerator.DisposeAsync();
            }
        }
        finally
        {
            throttler.Dispose();
            cancellation.Dispose();
        }

        static async Task<TReturn> ExecuteElementAsync(T element, Func<T, CancellationToken, Task<TReturn>> action, Action cancel, CancellationToken innerToken, Func<int> release)
        {
            try
            {
                return await action(element, innerToken);
            }
            catch
            {
                cancel();
                throw;
            }
            finally
            {
                release();
            }
        }
    }

    private static async Task WaitForRunningTasksAsync(IEnumerable<Task> tasks)
    {
        try
        {
            await Task.WhenAll(tasks);
        }
        catch
        {
            // Preserve the original producer/cancellation failure.
        }
    }

    private static void ValidateArguments(object source, Delegate action, int concurrentThreads)
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