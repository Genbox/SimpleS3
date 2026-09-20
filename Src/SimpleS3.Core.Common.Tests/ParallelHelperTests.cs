using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Common.Tests;

[SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Concurrency tests deliberately coordinate pre-started operations with test-controlled task signals.")]
public class ParallelHelperTests
{
    [Fact]
    public async Task ExecuteAsyncDoesNotOverReleaseWhenWaitIsCanceledAsync()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        TaskCompletionSource entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        int started = 0;

        Task task = ParallelHelper.ExecuteAsync([1, 2], async (_, _) =>
        {
            Interlocked.Increment(ref started);
            entered.TrySetResult();
            await release.Task.WaitAsync(TestContext.Current.CancellationToken);
        }, 1, cts.Token);

        await entered.Task.WaitAsync(TestContext.Current.CancellationToken);
        await cts.CancelAsync();
        release.SetResult();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task.WaitAsync(TestContext.Current.CancellationToken));
        Assert.Equal(1, Volatile.Read(ref started));
    }

    [Fact]
    public async Task ExecuteAsyncWithReturnReleasesSlotWhenActionThrowsSynchronously() => await Assert.ThrowsAsync<InvalidOperationException>(() => ParallelHelper.ExecuteAsync([1, 2], (value, _) =>
    {
        if (value == 1)
            throw new InvalidOperationException();

        return Task.FromResult(value);
    }, 1, TestContext.Current.CancellationToken));

    [Fact]
    public async Task ExecuteAsyncWithReturnDoesNotEnumeratePastAvailableSlotsAsync()
    {
        int yielded = 0;
        TaskCompletionSource entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task<IEnumerable<int>> task = ParallelHelper.ExecuteAsync(Source(), async (value, token) =>
        {
            entered.TrySetResult();
            await release.Task.WaitAsync(token);
            return value;
        }, 1, TestContext.Current.CancellationToken);

        await entered.Task.WaitAsync(TestContext.Current.CancellationToken);

        Assert.Equal(1, Volatile.Read(ref yielded));

        release.SetResult();
        Assert.Equal([0, 1, 2], await task);

        IEnumerable<int> Source()
        {
            for (int i = 0; i < 3; i++)
            {
                Interlocked.Increment(ref yielded);
                yield return i;
            }
        }
    }

    [Fact]
    public async Task ExecuteAsyncWithReturnCancelsSiblingsWhenActionThrowsAsync()
    {
        int started = 0;
        int finished = 0;
        TaskCompletionSource siblingEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await Assert.ThrowsAsync<InvalidOperationException>(() => ParallelHelper.ExecuteAsync(Enumerable.Range(0, 10), async (value, token) =>
        {
            Interlocked.Increment(ref started);

            if (value == 0)
            {
                await siblingEntered.Task.WaitAsync(token);
                throw new InvalidOperationException();
            }

            siblingEntered.TrySetResult();

            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
                return value;
            }
            finally
            {
                Interlocked.Increment(ref finished);
            }
        }, 2, TestContext.Current.CancellationToken));

        Assert.Equal(2, Volatile.Read(ref started));
        Assert.Equal(1, Volatile.Read(ref finished));
    }

    [Fact]
    public async Task ExecuteAsyncWithReturnPreservesSourceOrderWhenWorkersFinishOutOfOrder()
    {
        TaskCompletionSource otherFinished = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        ConcurrentQueue<int> completionOrder = new ConcurrentQueue<int>();
        IEnumerable<int> results = await ParallelHelper.ExecuteAsync(Enumerable.Range(0, 100), async (value, token) =>
        {
            if (value == 0)
                await otherFinished.Task.WaitAsync(token);
            completionOrder.Enqueue(value);
            if (value != 0)
                otherFinished.TrySetResult();
            return value;
        }, 8, TestContext.Current.CancellationToken);

        Assert.Equal(100, completionOrder.Count);
        Assert.NotEqual(0, completionOrder.First());
        Assert.Equal(Enumerable.Range(0, 100), results);
    }

    [Fact]
    public async Task ExecuteAsyncCancelsRunningTasksWhenSourceThrowsAsync()
    {
        int finished = 0;
        TaskCompletionSource entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task task = ParallelHelper.ExecuteAsync(Source(), async (_, token) =>
        {
            entered.SetResult();

            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
            }
            finally
            {
                Interlocked.Increment(ref finished);
            }
        }, 2, TestContext.Current.CancellationToken);

        await entered.Task.WaitAsync(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(() => task.WaitAsync(TestContext.Current.CancellationToken));
        Assert.Equal(1, Volatile.Read(ref finished));

        static IEnumerable<int> Source()
        {
            yield return 1;
            throw new InvalidOperationException();
        }
    }

    [Fact]
    public async Task ExecuteAsyncWithAsyncSourceCancelsRunningTasksWhenSourceThrowsAsync()
    {
        int finished = 0;
        TaskCompletionSource entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task<IEnumerable<int>> task = ParallelHelper.ExecuteAsync(SourceAsync(TestContext.Current.CancellationToken), async (_, token) =>
        {
            entered.SetResult();

            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
            }
            finally
            {
                Interlocked.Increment(ref finished);
            }

            return 1;
        }, 2, TestContext.Current.CancellationToken);

        await entered.Task.WaitAsync(TestContext.Current.CancellationToken);

        await Assert.ThrowsAsync<InvalidOperationException>(() => task.WaitAsync(TestContext.Current.CancellationToken));
        Assert.Equal(1, Volatile.Read(ref finished));

        static async IAsyncEnumerable<int> SourceAsync([EnumeratorCancellation]CancellationToken token = default)
        {
            yield return 1;
            await Task.Yield();
            token.ThrowIfCancellationRequested();
            throw new InvalidOperationException();
        }
    }
}