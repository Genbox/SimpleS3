using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Responses.S3Types;
using Genbox.SimpleS3.Core.Responses.Service;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Service
{
    public class GetServiceTests : LiveTestBase
    {
        public GetServiceTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task CRUDTest()
        {
            string bucket1 = "testbucket-" + Guid.NewGuid();
            string bucket2 = "testbucket-" + Guid.NewGuid();

            await BucketClient.PutBucketAsync(bucket1, AwsRegion.EUWest1).ConfigureAwait(false);
            await BucketClient.PutBucketAsync(bucket2, AwsRegion.EUWest1).ConfigureAwait(false);

            List<S3Bucket> list = await ServiceClient.GetServiceAllAsync().ToListAsync().ConfigureAwait(false);

            Assert.True(list.Count >= 2); //Larger than because other tests might have run at the same time
            S3Bucket bucket1obj = Assert.Single(list, x => x.Name == bucket1);
            Assert.Equal(DateTime.UtcNow, bucket1obj.CreationDate.DateTime, TimeSpan.FromSeconds(5));

            S3Bucket bucket2obj = Assert.Single(list, x => x.Name == bucket2);
            Assert.Equal(DateTime.UtcNow, bucket2obj.CreationDate.DateTime, TimeSpan.FromSeconds(5));

            //Cleanup
            await BucketClient.DeleteBucketAsync(bucket1).ConfigureAwait(false);
            await BucketClient.DeleteBucketAsync(bucket2).ConfigureAwait(false);
        }

        [Fact]
        public async Task GetService()
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();
            await BucketClient.PutBucketAsync(tempBucketName, request => request.Region = Config.Region).ConfigureAwait(false);

            GetServiceResponse resp = await ServiceClient.GetServiceAsync().ConfigureAwait(false);
            Assert.True(resp.Buckets.Count > 0);

            Assert.NotNull(resp.Buckets.SingleOrDefault(x => x.Name == tempBucketName));
        }
    }
}