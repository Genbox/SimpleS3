using System.Runtime.CompilerServices;
using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Common.Tests;

public class ParallelHelperTests
{
    [Fact]
    public async Task ExecuteAsyncDoesNotOverReleaseWhenWaitIsCanceled()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        TaskCompletionSource entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task task = ParallelHelper.ExecuteAsync([1, 2], async (_, token) =>
        {
            entered.SetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, token);
        }, 1, cts.Token);

        await entered.Task;
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    }

    [Fact]
    public async Task ExecuteAsyncWithReturnReleasesSlotWhenActionThrowsSynchronously()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => ParallelHelper.ExecuteAsync([1, 2], (value, _) =>
        {
            if (value == 1)
                throw new InvalidOperationException();

            return Task.FromResult(value);
        }, 1, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExecuteAsyncWithReturnDoesNotEnumeratePastAvailableSlots()
    {
        int yielded = 0;
        TaskCompletionSource entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task<IEnumerable<int>> task = ParallelHelper.ExecuteAsync(Source(), async (value, token) =>
        {
            entered.TrySetResult();
            await release.Task.WaitAsync(token).ConfigureAwait(false);
            return value;
        }, 1, TestContext.Current.CancellationToken);

        await entered.Task.WaitAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);

        Assert.Equal(1, Volatile.Read(ref yielded));

        release.SetResult();
        Assert.Equal([0, 1, 2], await task.ConfigureAwait(false));

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
    public async Task ExecuteAsyncWithReturnCancelsSiblingsWhenActionThrows()
    {
        int started = 0;

        await Assert.ThrowsAsync<InvalidOperationException>(() => ParallelHelper.ExecuteAsync(Enumerable.Range(0, 10), async (value, token) =>
        {
            Interlocked.Increment(ref started);

            if (value == 0)
                throw new InvalidOperationException();

            await Task.Delay(Timeout.InfiniteTimeSpan, token).ConfigureAwait(false);
            return value;
        }, 2, TestContext.Current.CancellationToken));

        Assert.True(Volatile.Read(ref started) < 10);
    }

    [Fact]
    public async Task ExecuteAsyncCancelsRunningTasksWhenSourceThrows()
    {
        TaskCompletionSource entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task task = ParallelHelper.ExecuteAsync(Source(), async (_, token) =>
        {
            entered.SetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, token).ConfigureAwait(false);
        }, 2, TestContext.Current.CancellationToken);

        await entered.Task.WaitAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => task.WaitAsync(TestContext.Current.CancellationToken));

        static IEnumerable<int> Source()
        {
            yield return 1;
            throw new InvalidOperationException();
        }
    }

    [Fact]
    public async Task ExecuteAsyncWithAsyncSourceCancelsRunningTasksWhenSourceThrows()
    {
        TaskCompletionSource entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task<IEnumerable<int>> task = ParallelHelper.ExecuteAsync(Source(TestContext.Current.CancellationToken), async (_, token) =>
        {
            entered.SetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, token).ConfigureAwait(false);
            return 1;
        }, 2, TestContext.Current.CancellationToken);

        await entered.Task.WaitAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => task.WaitAsync(TestContext.Current.CancellationToken));

        static async IAsyncEnumerable<int> Source([EnumeratorCancellation]CancellationToken token = default)
        {
            yield return 1;
            await Task.Yield();
            token.ThrowIfCancellationRequested();
            throw new InvalidOperationException();
        }
    }
}