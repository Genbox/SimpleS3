using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.LiveTests.Buckets
{
    public class BucketAccelerateConfigurationTests : LiveTestBase
    {
        public BucketAccelerateConfigurationTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task PutGetBucketAccelerateConfiguration()
        {
            await CreateTempBucketAsync(async x =>
            {
                GetBucketAccelerateConfigurationResponse getResp = await BucketClient.GetBucketAccelerateConfigurationAsync(x).ConfigureAwait(false);
                Assert.True(getResp.IsSuccess);
                Assert.False(getResp.AccelerateEnabled);

                PutBucketAccelerateConfigurationResponse putResp = await BucketClient.PutBucketAccelerateConfigurationAsync(x, true).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

                getResp = await BucketClient.GetBucketAccelerateConfigurationAsync(x).ConfigureAwait(false);
                Assert.True(getResp.IsSuccess);
                Assert.True(getResp.AccelerateEnabled);
            }).ConfigureAwait(false);
        }
    }
}