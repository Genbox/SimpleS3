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
}