using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Responses.S3Types;
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
            await BucketClient.CreateBucketAsync(tempBucketName).ConfigureAwait(false);

            List<S3Bucket> list = await S3BucketClientExtensions.ListBucketsAsync(BucketClient).ToListAsync().ConfigureAwait(false);
            Assert.True(list.Count > 0);

            Assert.NotNull(list.SingleOrDefault(x => x.Name == tempBucketName));
        }
    }
}