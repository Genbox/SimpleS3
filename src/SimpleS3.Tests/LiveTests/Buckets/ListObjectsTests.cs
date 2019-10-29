using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.S3Types;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Buckets
{
    public class ListObjectsTests : LiveTestBase
    {
        public ListObjectsTests(ITestOutputHelper helper) : base(helper)
        {
        }

        private async Task UploadString(string bucketName, string objName, SemaphoreSlim semaphore)
        {
            try
            {
                await ObjectClient.PutObjectStringAsync(bucketName, objName, string.Empty).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }

        [Fact]
        public async Task GetWithOwner()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                await ObjectClient.PutObjectStringAsync(bucket, tempObjName, string.Empty, config: request => request.AclGrantFullControl.AddEmail(TestConstants.TestEmail)).ConfigureAwait(false);

                ListObjectsResponse gResp = await BucketClient.ListObjectsAsync(bucket, request => request.FetchOwner = true).ConfigureAwait(false);
                Assert.True(gResp.IsSuccess);
                Assert.Equal(1, gResp.KeyCount);

                S3Object obj = gResp.Objects.First();
                Assert.Equal(TestConstants.TestUsername, obj.Owner.Name);
                Assert.Equal(TestConstants.TestUserId, obj.Owner.Id);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task GetWithPrefix()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                string tempObjName2 = "something-" + Guid.NewGuid();

                await ObjectClient.PutObjectStringAsync(bucket, tempObjName, "hello").ConfigureAwait(false);
                await ObjectClient.PutObjectStringAsync(bucket, tempObjName2, "world!").ConfigureAwait(false);

                ListObjectsResponse gResp = await BucketClient.ListObjectsAsync(bucket, request => request.Prefix = "object").ConfigureAwait(false);
                Assert.True(gResp.IsSuccess);

                Assert.Equal(1, gResp.KeyCount);
                Assert.Equal(1, gResp.Objects.Count);

                Assert.Equal("object", gResp.Prefix);

                Assert.Equal(tempObjName, gResp.Objects[0].ObjectKey);
                Assert.Equal(DateTime.UtcNow, gResp.Objects[0].LastModified.DateTime, TimeSpan.FromSeconds(5));
                Assert.Equal("\"5d41402abc4b2a76b9719d911017c592\"", gResp.Objects[0].ETag);
                Assert.Equal(5, gResp.Objects[0].Size);
                Assert.Equal(StorageClass.Standard, gResp.Objects[0].StorageClass);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListWithDelimiter()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                string tempObjName2 = "something-" + Guid.NewGuid();

                await ObjectClient.PutObjectStringAsync(bucket, tempObjName, "hello").ConfigureAwait(false);
                await ObjectClient.PutObjectStringAsync(bucket, tempObjName2, "world!").ConfigureAwait(false);

                ListObjectsResponse gResp = await BucketClient.ListObjectsAsync(bucket, request => request.Delimiter = "-").ConfigureAwait(false);
                Assert.True(gResp.IsSuccess);

                Assert.Equal(2, gResp.KeyCount);
                Assert.Equal(2, gResp.CommonPrefixes.Count);
                Assert.Equal("object-", gResp.CommonPrefixes[0]);
                Assert.Equal("something-", gResp.CommonPrefixes[1]);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task MoreThanMaxKeys()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                int concurrent = 10;
                int count = 1001;
                Queue<Task> tasks = new Queue<Task>(count);

                using (SemaphoreSlim semaphore = new SemaphoreSlim(concurrent))
                {
                    for (int i = 0; i < count; i++)
                    {
                        await semaphore.WaitAsync().ConfigureAwait(false);

                        string tempObjName = "object-" + Guid.NewGuid();
                        tasks.Enqueue(UploadString(bucket, tempObjName, semaphore));
                    }

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }

                ListObjectsResponse gResp = await BucketClient.ListObjectsAsync(bucket).ConfigureAwait(false);
                Assert.True(gResp.IsSuccess);
                Assert.Equal(1000, gResp.KeyCount);
                Assert.Equal(1000, gResp.Objects.Count);
                Assert.NotEmpty(gResp.NextContinuationToken);
                Assert.True(gResp.IsTruncated);
            }).ConfigureAwait(false);
        }
    }
}