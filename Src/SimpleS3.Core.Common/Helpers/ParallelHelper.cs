using System.Diagnostics.CodeAnalysis;

namespace Genbox.SimpleS3.Core.Common.Helpers;

/// <summary>Executes asynchronous operations with bounded concurrency and coordinated cancellation.</summary>
[SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP013:Await in using", Justification = "All overloads await their workers before disposing the enumerator and coordination resources.")]
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
        using IEnumerator<T> enumerator = source.GetEnumerator();
        CancellationToken innerToken = cancellation.Token;
        Action cancel = cancellation.Cancel;
        object enumerationLock = new object();
        Task[] workers = [.. Enumerable.Range(0, concurrentThreads).Select(_ => ExecuteWorkerAsync())];
        await Task.WhenAll(workers);

        async Task ExecuteWorkerAsync()
        {
            try
            {
                while (true)
                {
                    innerToken.ThrowIfCancellationRequested();

                    T element;

                    lock (enumerationLock)
                    {
                        if (!enumerator.MoveNext())
                            return;

                        element = enumerator.Current;
                    }

                    await action(element, innerToken);
                }
            }
            catch
            {
                cancel();
                throw;
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
        using IEnumerator<T> enumerator = source.GetEnumerator();
        CancellationToken innerToken = cancellation.Token;
        object stateLock = new object();
        List<TReturn> results = [];
        Task[] workers = [.. Enumerable.Range(0, concurrentThreads).Select(_ => ExecuteWorkerAsync())];
        await Task.WhenAll(workers);
        return results;

        async Task ExecuteWorkerAsync()
        {
            try
            {
                while (true)
                {
                    innerToken.ThrowIfCancellationRequested();

                    T element;
                    int index;

                    lock (stateLock)
                    {
                        if (!enumerator.MoveNext())
                            return;

                        element = enumerator.Current;
                        index = results.Count;
                        results.Add(default!);
                    }

                    TReturn result = await action(element, innerToken);

                    lock (stateLock)
                        results[index] = result;
                }
            }
            catch
            {
                cancellation.Cancel();
                throw;
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

        using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
        using SemaphoreSlim enumerationLock = new SemaphoreSlim(1, 1);
        IAsyncEnumerator<T> enumerator = source.GetAsyncEnumerator(cancellation.Token);
        CancellationToken innerToken = cancellation.Token;
        object resultsLock = new object();
        List<TReturn> results = [];

        try
        {
            Task[] workers = [.. Enumerable.Range(0, concurrentThreads).Select(_ => ExecuteWorkerAsync())];
            await Task.WhenAll(workers);
            return results;
        }
        finally
        {
            await enumerator.DisposeAsync();
        }

        async Task ExecuteWorkerAsync()
        {
            try
            {
                while (true)
                {
                    innerToken.ThrowIfCancellationRequested();
                    T element;
                    int index;

                    await enumerationLock.WaitAsync(innerToken);

                    try
                    {
                        if (!await enumerator.MoveNextAsync())
                            return;

                        element = enumerator.Current;

                        lock (resultsLock)
                        {
                            index = results.Count;
                            results.Add(default!);
                        }
                    }
                    finally
                    {
                        enumerationLock.Release();
                    }

                    TReturn result = await action(element, innerToken);

                    lock (resultsLock)
                        results[index] = result;
                }
            }
            catch
            {
                cancellation.Cancel();
                throw;
            }
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
}