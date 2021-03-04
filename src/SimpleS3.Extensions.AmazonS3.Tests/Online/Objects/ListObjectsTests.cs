using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.TestBase;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Tests.Online.Objects
{
    public class ListObjectsTests : AwsTestBase
    {
        public ListObjectsTests(ITestOutputHelper helper) : base(helper) { }

        [Fact]
        public async Task ListObjects()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                await UploadAsync(bucket, tempObjName, "hello").ConfigureAwait(false);

                ListObjectsResponse gResp = await ObjectClient.ListObjectsAsync(bucket).ConfigureAwait(false);
                Assert.True(gResp.IsSuccess);

                Assert.Equal(bucket, gResp.BucketName);
                Assert.Equal(1, gResp.KeyCount);
                Assert.Equal(1000, gResp.MaxKeys);
                Assert.False(gResp.IsTruncated);

                S3Object obj = gResp.Objects.First();
                Assert.Equal(tempObjName, obj.ObjectKey);
                Assert.Equal("\"5d41402abc4b2a76b9719d911017c592\"", obj.ETag);
                Assert.Equal(StorageClass.Standard, obj.StorageClass);
                Assert.Equal(5, obj.Size);
                Assert.Equal(DateTime.UtcNow, obj.LastModifiedOn.UtcDateTime, TimeSpan.FromSeconds(5));
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListObjectsMoreThanMaxKeys()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                int concurrent = 10;
                int count = 11;

                await ParallelHelper.ExecuteAsync(Enumerable.Range(0, count), i => UploadAsync(bucket, i.ToString()), concurrent, CancellationToken.None);

                ListObjectsResponse resp = await ObjectClient.ListObjectsAsync(bucket, req => req.MaxKeys = count - 1).ConfigureAwait(false);
                Assert.True(resp.IsSuccess);

                Assert.Equal(count - 1, resp.KeyCount);
                Assert.Equal(count - 1, resp.Objects.Count);
                Assert.NotEmpty(resp.NextContinuationToken);
                Assert.True(resp.IsTruncated);

                ListObjectsResponse resp2 = await ObjectClient.ListObjectsAsync(bucket, req => req.ContinuationToken = resp.NextContinuationToken).ConfigureAwait(false);
                Assert.True(resp2.IsSuccess);

                Assert.Equal(1, resp2.KeyCount);
                Assert.Equal(1, resp2.Objects.Count);
                Assert.Equal(resp.NextContinuationToken, resp2.ContinuationToken);
                Assert.Null(resp2.NextContinuationToken);
                Assert.False(resp2.IsTruncated);
            }).ConfigureAwait(false);
        }

        [Fact(Skip = "Require a setup of another AWS account with 'Requester pays' setup")]
        public async Task ListObjectsRequestPayer()
        {
            ListObjectsResponse resp = await ObjectClient.ListObjectsAsync(BucketName, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(resp.RequestCharged);
        }

        [Fact]
        public async Task ListObjectsWithDelimiter()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                string tempObjName2 = "something-" + Guid.NewGuid();

                await UploadAsync(bucket, tempObjName).ConfigureAwait(false);
                await UploadAsync(bucket, tempObjName2).ConfigureAwait(false);

                ListObjectsResponse resp = await ObjectClient.ListObjectsAsync(bucket, req => req.Delimiter = "-").ConfigureAwait(false);
                Assert.True(resp.IsSuccess);

                Assert.Equal("-", resp.Delimiter);
                Assert.Equal(2, resp.KeyCount);
                Assert.Equal(2, resp.CommonPrefixes!.Count);
                Assert.Equal("object-", resp.CommonPrefixes[0]);
                Assert.Equal("something-", resp.CommonPrefixes[1]);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListObjectsWithEncoding()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "!#/()";

                await UploadAsync(bucket, tempObjName).ConfigureAwait(false);

                ListObjectsResponse resp = await ObjectClient.ListObjectsAsync(bucket, req => req.EncodingType = EncodingType.Url).ConfigureAwait(false);
                Assert.True(resp.IsSuccess);

                Assert.Equal(EncodingType.Url, resp.EncodingType);

                S3Object? obj = Assert.Single(resp.Objects);

                Assert.Equal("%21%23/%28%29", obj.ObjectKey);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListObjectsWithOwner()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                await UploadAsync(bucket, tempObjName, req => req.AclGrantFullControl.AddEmail(TestConstants.TestEmail)).ConfigureAwait(false);

                ListObjectsResponse resp = await ObjectClient.ListObjectsAsync(bucket, req => req.FetchOwner = true).ConfigureAwait(false);
                Assert.True(resp.IsSuccess);

                S3Object obj = resp.Objects.First();
                Assert.Equal(TestConstants.TestUsername, obj.Owner!.Name);
                Assert.Equal(TestConstants.TestUserId, obj.Owner.Id);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListObjectsWithPrefix()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                string tempObjName2 = "something-" + Guid.NewGuid();

                await UploadAsync(bucket, tempObjName).ConfigureAwait(false);
                await UploadAsync(bucket, tempObjName2).ConfigureAwait(false);

                ListObjectsResponse resp = await ObjectClient.ListObjectsAsync(bucket, req => req.Prefix = "object").ConfigureAwait(false);
                Assert.True(resp.IsSuccess);

                Assert.Equal(1, resp.KeyCount);
                Assert.Equal("object", resp.Prefix);

                S3Object? obj = Assert.Single(resp.Objects);

                Assert.Equal(tempObjName, obj.ObjectKey);
            }).ConfigureAwait(false);
        }
    }
}