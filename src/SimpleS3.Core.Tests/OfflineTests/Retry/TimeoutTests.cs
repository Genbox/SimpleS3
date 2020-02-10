using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Tests.Code.Other;
using Genbox.SimpleS3.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests.Retry
{
    /// <summary>
    /// Tests when the requests time out
    /// </summary>
    public class TimeoutTests : OfflineTestBase
    {
        private readonly BaseFailingHttpHandler _handler = new SlowHttpHandler(2, TimeSpan.FromSeconds(5));

        public TimeoutTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        protected override void ConfigureClientBuilder(IClientBuilder builder)
        {
            builder.UseHttpClientFactory()
                .ConfigurePrimaryHttpMessageHandler(() => _handler)
                .AddRetryPolicy(3, attempt => TimeSpan.Zero)
                // Set an extraordinary timeout
                .AddTimeoutPolicy(TimeSpan.FromSeconds(3));
        }

        [Fact]
        public async Task TestClientCancellationToken()
        {
            using MemoryStream ms = new MemoryStream(new byte[4096]);
            using CancellationTokenSource tcs = new CancellationTokenSource();

            var task = ObjectClient.PutObjectAsync(BucketName, nameof(TestClientCancellationToken), ms, token: tcs.Token);
            tcs.CancelAfter(500);

            Stopwatch sw = Stopwatch.StartNew();
            await Assert.ThrowsAsync<TaskCanceledException>(async () => await task.ConfigureAwait(false)).ConfigureAwait(false);
            sw.Stop();

            // We should have canceled within 750ms
            Assert.True(sw.ElapsedMilliseconds < 750);

            // TODO: Be able to confirm with the TaskCanceledException, that our TCS was the reason for it
        }

        [Fact]
        public async Task TestTimeoutError()
        {
            using MemoryStream ms = new MemoryStream(new byte[4096]);

            // TODO: Evaluate why this throws TaskCanceledException
            PutObjectResponse response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestTimeoutError), ms).ConfigureAwait(false);

            // Request should succeed after N tries
            Assert.True(response.IsSuccess);
            Assert.Equal(2, _handler.RequestCounter);
        }

        [Fact]
        public async Task TestTimeoutError_NonSeekableStream()
        {
            using NonSeekableStream ms = new NonSeekableStream(new byte[4096]);

            // TODO: Evaluate why this throws TaskCanceledException
            PutObjectResponse response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestTimeoutError_NonSeekableStream), ms).ConfigureAwait(false);

            // Request should succeed after N tries
            Assert.True(response.IsSuccess);
            Assert.Equal(2, _handler.RequestCounter);
        }
    }
}