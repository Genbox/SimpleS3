using System;
using System.IO;
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
    /// Tests when the network does not work
    /// </summary>
    public class NetworkErrorTests : OfflineTestBase
    {
        private readonly BaseFailingHttpHandler _handler = new NetworkFailingHttpHandler(1);

        public NetworkErrorTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        protected override void ConfigureClientBuilder(IClientBuilder builder)
        {
            builder.UseHttpClientFactory()
                .ConfigurePrimaryHttpMessageHandler(() => _handler)
                .AddRetryPolicy(3, attempt => TimeSpan.Zero);
        }

        [Fact]
        public async Task TestNonTransientNetworkError()
        {
            using MemoryStream ms = new MemoryStream(new byte[4096]);

            // One request should succeed
            PutObjectResponse response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestNonTransientNetworkError) + "-0", ms).ConfigureAwait(false);

            Assert.True(response.IsSuccess);
            Assert.Equal(1, _handler.RequestCounter);

            // Second request should fail with a network error
            await Assert.ThrowsAsync<IOException>(async () => await ObjectClient.PutObjectAsync(BucketName, nameof(TestNonTransientNetworkError) + "-1", ms).ConfigureAwait(false)).ConfigureAwait(false);

            // Because network errors are transient, they should be retried
            Assert.Equal(5, _handler.RequestCounter);
        }
    }
}