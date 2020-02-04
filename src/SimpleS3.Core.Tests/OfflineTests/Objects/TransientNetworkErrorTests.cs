using System.IO;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Tests.Code.Other;
using Genbox.SimpleS3.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests.Objects
{
    public class TransientNetworkErrorTests : OfflineTestBase
    {
        private readonly TransientFailingHttpHandler _handler = new TransientFailingHttpHandler();

        public TransientNetworkErrorTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        protected override void ConfigureClientBuilder(IClientBuilder builder)
        {
            builder.UseHttpClientFactory()
                .ConfigurePrimaryHttpMessageHandler(() => _handler)
                .AddDefaultRetryPolicy();
        }

        protected override void ConfigureServices(IServiceCollection collection)
        {
            // TODO: Add this first
            // TODO: Add this in DefaultRetryPolicy
            collection.AddSingleton<IRequestStreamWrapper, RetryableBufferingStreamWrapper>();
        }

        [Fact]
        public async Task TestTransientNetworkError()
        {
            var ms = new MemoryStream(new byte[4096]);

            var response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestTransientNetworkError), ms).ConfigureAwait(false);

            Assert.True(response.IsSuccess);
            Assert.True(_handler.RequestCounter >= 2);
        }

        [Fact]
        public async Task TestTransientNetworkError_Nonseekable()
        {
            var ms = new NonSeekableStream(new byte[4096]);

            var response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestTransientNetworkError_Nonseekable), ms).ConfigureAwait(false);

            Assert.True(response.IsSuccess);
            Assert.True(_handler.RequestCounter >= 2);
        }
    }
}