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
    public class TransientNetworkErrorTests : OfflineTestBase
    {
        private readonly BaseFailingHttpHandler _handler = new TransientFailingHttpHandler();

        public TransientNetworkErrorTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        protected override void ConfigureCoreBuilder(ICoreBuilder builder)
        {
            builder.UseHttpClientFactory()
                   .ConfigurePrimaryHttpMessageHandler(() => _handler)
                   .UseRetryPolicy(3, attempt => TimeSpan.Zero);
        }

        [Fact]
        public async Task TestTransientNetworkError()
        {
            using MemoryStream ms = new MemoryStream(new byte[4096]);

            PutObjectResponse response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestTransientNetworkError), ms).ConfigureAwait(false);

            Assert.True(response.IsSuccess);
            Assert.True(_handler.RequestCounter >= 2);
        }

        [Fact]
        public async Task TestTransientNetworkError_Nonseekable()
        {
            using NonSeekableStream ms = new NonSeekableStream(new byte[4096]);

            PutObjectResponse response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestTransientNetworkError_Nonseekable), ms).ConfigureAwait(false);

            Assert.True(response.IsSuccess);
            Assert.True(_handler.RequestCounter >= 2);
        }
    }
}