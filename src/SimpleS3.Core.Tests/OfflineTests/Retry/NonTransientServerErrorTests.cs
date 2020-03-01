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
    /// Tests when the network works, but the API is responding with server errors
    /// </summary>
    public class NonTransientServerErrorTests : OfflineTestBase
    {
        private readonly BaseFailingHttpHandler _handler = new NonTransientFailingHttpHandler(1);

        public NonTransientServerErrorTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        protected override void ConfigureCoreBuilder(ICoreBuilder builder)
        {
            builder.UseHttpClientFactory()
                   .ConfigurePrimaryHttpMessageHandler(() => _handler)
                   .UseRetryPolicy(3, attempt => TimeSpan.Zero);
        }

        [Fact]
        public async Task TestNonTransientServerError()
        {
            using MemoryStream ms = new MemoryStream(new byte[4096]);

            // One request should succeed
            PutObjectResponse response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestNonTransientServerError) + "-0", ms).ConfigureAwait(false);

            Assert.True(response.IsSuccess);
            Assert.Equal(1, _handler.RequestCounter);

            // Second request should fail
            response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestNonTransientServerError) + "-1", ms).ConfigureAwait(false);

            Assert.False(response.IsSuccess);
            Assert.Equal(2, _handler.RequestCounter);
        }
    }
}