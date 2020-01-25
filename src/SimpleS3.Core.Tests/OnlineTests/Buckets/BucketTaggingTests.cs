using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Buckets
{
    public class BucketTaggingTests : OnlineTestBase
    {
        public BucketTaggingTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task GetPutDeleteBucketTagging()
        {
            await CreateTempBucketAsync(async x =>
            {
                IDictionary<string, string> tags = new Dictionary<string, string>();
                tags.Add("MyKey", "MyValue");
                tags.Add("MyKey2", "MyValue2");

                PutBucketTaggingResponse putResp = await BucketClient.PutBucketTaggingAsync(x, tags).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

                GetBucketTaggingResponse getResp = await BucketClient.GetBucketTaggingAsync(x).ConfigureAwait(false);
                Assert.True(getResp.IsSuccess);

                Assert.Equal(tags, getResp.Tags);

                DeleteBucketTaggingResponse deleteResp = await BucketClient.DeleteBucketTaggingAsync(x).ConfigureAwait(false);
                Assert.True(deleteResp.IsSuccess);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task GetEmptyBucketTagging()
        {
            await CreateTempBucketAsync(async x =>
            {
                GetBucketTaggingResponse getResp = await BucketClient.GetBucketTaggingAsync(x).ConfigureAwait(false);
                Assert.Equal(404, getResp.StatusCode);
            }).ConfigureAwait(false);
        }
    }
}