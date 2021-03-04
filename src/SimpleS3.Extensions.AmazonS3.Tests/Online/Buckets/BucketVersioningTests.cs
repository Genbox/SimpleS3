using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Tests.Online.Buckets
{
    public class BucketVersioningTests : AwsTestBase
    {
        public BucketVersioningTests(ITestOutputHelper helper) : base(helper) { }

        [Fact]
        public async Task PutBucketVersioningRequest()
        {
            await CreateTempBucketAsync(async x =>
            {
                //Check if versioning is enabled (it shouldn't be)
                GetBucketVersioningResponse getResp = await BucketClient.GetBucketVersioningAsync(x);
                Assert.True(getResp.IsSuccess);
                Assert.False(getResp.Status);
                Assert.False(getResp.MfaDelete);

                //Enable versioning
                PutBucketVersioningResponse putResp = await BucketClient.PutBucketVersioningAsync(x, true);
                Assert.True(putResp.IsSuccess);

                //Check if versioning is enabled (it should be)
                getResp = await BucketClient.GetBucketVersioningAsync(x);
                Assert.True(getResp.IsSuccess);
                Assert.True(getResp.Status);

                //Disable versioning
                putResp = await BucketClient.PutBucketVersioningAsync(x, false);
                Assert.True(putResp.IsSuccess);

                //Check if versioning is enabled (it shouldn't be)
                getResp = await BucketClient.GetBucketVersioningAsync(x);
                Assert.True(getResp.IsSuccess);
                Assert.False(getResp.Status);
            }).ConfigureAwait(false);
        }
    }
}