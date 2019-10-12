using System;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Buckets
{
    public class ListBuckets : LiveTestBase
    {
        public ListBuckets(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task ListBucketsTest()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();
            await BucketClient.CreateBucketAsync(tempBucketName, request => request.Region = Config.Region).ConfigureAwait(false);

            ListBucketsResponse resp = await BucketClient.ListBucketsAsync().ConfigureAwait(false);
            Assert.True(resp.Buckets.Count > 0);

            Assert.NotNull(resp.Buckets.SingleOrDefault(x => x.Name == tempBucketName));
        }
    }
}