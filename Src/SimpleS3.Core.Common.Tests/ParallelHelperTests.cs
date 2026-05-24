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
}