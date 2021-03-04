using System;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.TestBase;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Tests.Online.Objects
{
    public class ListObjectVersionsTests : AwsTestBase
    {
        public ListObjectVersionsTests(ITestOutputHelper helper) : base(helper) { }

        [Fact]
        public async Task ListObjectVersions()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                //Enable versioning on the bucket
                await BucketClient.PutBucketVersioningAsync(bucket, true);

                //Verify that we enabled bucket versioning
                GetBucketVersioningResponse getVerResp = await BucketClient.GetBucketVersioningAsync(bucket);
                Assert.True(getVerResp.Status);

                PutObjectResponse putResp1 = await UploadAsync(bucket, "1", "a").ConfigureAwait(false);
                PutObjectResponse putResp2 = await UploadAsync(bucket, "2", "aa").ConfigureAwait(false);
                PutObjectResponse putResp3 = await UploadAsync(bucket, "3", "aaa").ConfigureAwait(false);

                DeleteObjectResponse putResp4 = await ObjectClient.DeleteObjectAsync(bucket, "2"); //Delete object 2
                PutObjectResponse putResp5 = await UploadAsync(bucket, "3", "aaaa").ConfigureAwait(false); //Overwrite object 3

                ListObjectVersionsResponse listResp = await ObjectClient.ListObjectVersionsAsync(bucket);
                Assert.True(listResp.IsSuccess);
                Assert.Equal(4, listResp.Versions.Count);
                Assert.Equal(1, listResp.DeleteMarkers.Count);

                Assert.Equal(bucket, listResp.Name);
                Assert.Equal(string.Empty, listResp.Prefix);
                Assert.Equal(string.Empty, listResp.KeyMarker);
                Assert.Equal(string.Empty, listResp.VersionIdMarker);
                Assert.Equal(1000, listResp.MaxKeys);
                Assert.False(listResp.IsTruncated);

                S3Version version1 = listResp.Versions[0];
                Assert.Equal("1", version1.ObjectKey);
                Assert.Equal(putResp1.VersionId, version1.VersionId);
                Assert.True(version1.IsLatest);
                Assert.Equal(DateTimeOffset.UtcNow.DateTime, version1.LastModified.DateTime, TimeSpan.FromMinutes(1));
                Assert.Equal("\"0cc175b9c0f1b6a831c399e269772661\"", version1.Etag);
                Assert.Equal(1, version1.Size);
                Assert.Equal(StorageClass.Standard, version1.StorageClass);
                Assert.Equal(TestConstants.TestUserId, version1.Owner.Id);
                Assert.Equal(TestConstants.TestUsername, version1.Owner.Name);

                S3Version version2 = listResp.Versions[1];
                Assert.Equal("2", version2.ObjectKey);
                Assert.Equal(putResp2.VersionId, version2.VersionId);
                Assert.False(version2.IsLatest);
                Assert.Equal(DateTimeOffset.UtcNow.DateTime, version2.LastModified.DateTime, TimeSpan.FromMinutes(1));
                Assert.Equal("\"4124bc0a9335c27f086f24ba207a4912\"", version2.Etag);
                Assert.Equal(2, version2.Size);
                Assert.Equal(StorageClass.Standard, version2.StorageClass);
                Assert.Equal(TestConstants.TestUserId, version2.Owner.Id);
                Assert.Equal(TestConstants.TestUsername, version2.Owner.Name);

                //This is the latest version of object 3 and should be 4 in size
                S3Version version3 = listResp.Versions[2];
                Assert.Equal("3", version3.ObjectKey);
                Assert.Equal(putResp5.VersionId, version3.VersionId);
                Assert.True(version3.IsLatest);
                Assert.Equal(DateTimeOffset.UtcNow.DateTime, version3.LastModified.DateTime, TimeSpan.FromMinutes(1));
                Assert.Equal("\"74b87337454200d4d33f80c4663dc5e5\"", version3.Etag);
                Assert.Equal(4, version3.Size);
                Assert.Equal(StorageClass.Standard, version3.StorageClass);
                Assert.Equal(TestConstants.TestUserId, version3.Owner.Id);
                Assert.Equal(TestConstants.TestUsername, version3.Owner.Name);

                //This was the previous version of object 3, so it should not be the latest and have 3 in size
                S3Version version3a = listResp.Versions[3];
                Assert.Equal("3", version3a.ObjectKey);
                Assert.Equal(putResp3.VersionId, version3a.VersionId);
                Assert.False(version3a.IsLatest);
                Assert.Equal(DateTimeOffset.UtcNow.DateTime, version3a.LastModified.DateTime, TimeSpan.FromMinutes(1));
                Assert.Equal("\"47bce5c74f589f4867dbd57e9ca9f808\"", version3a.Etag);
                Assert.Equal(3, version3a.Size);
                Assert.Equal(StorageClass.Standard, version3a.StorageClass);
                Assert.Equal(TestConstants.TestUserId, version3a.Owner.Id);
                Assert.Equal(TestConstants.TestUsername, version3a.Owner.Name);

                //This is the latest version of object 2, since it was deleted
                S3DeleteMarker delMarker = listResp.DeleteMarkers[0];
                Assert.True(delMarker.IsLatest);
                Assert.Equal("2", delMarker.ObjectKey);
                Assert.Equal(putResp4.VersionId, delMarker.VersionId);
                Assert.Equal(DateTimeOffset.UtcNow.DateTime, delMarker.LastModified.DateTime, TimeSpan.FromMinutes(1));
                Assert.Equal(TestConstants.TestUserId, delMarker.Owner.Id);
                Assert.Equal(TestConstants.TestUsername, delMarker.Owner.Name);

            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListObjectsMoreThanMaxKeys()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                int concurrent = 10;
                int count = 11;

                await ParallelHelper.ExecuteAsync(Enumerable.Range(0, count), i => UploadAsync(bucket, i.ToString()), concurrent);

                ListObjectVersionsResponse listResp = await ObjectClient.ListObjectVersionsAsync(bucket, req => req.MaxKeys = count - 1).ConfigureAwait(false);
                Assert.True(listResp.IsSuccess);
                Assert.True(listResp.IsTruncated);
                Assert.Equal(10, listResp.MaxKeys);
                Assert.Equal(10, listResp.Versions.Count);

                ListObjectVersionsResponse listResp2 = await ObjectClient.ListObjectVersionsAsync(bucket, req => req.KeyMarker = listResp.NextKeyMarker).ConfigureAwait(false);
                Assert.True(listResp2.IsSuccess);
                Assert.False(listResp2.IsTruncated);
                Assert.Equal(1, listResp2.Versions.Count);

            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListObjectVersionsWithDelimiter()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "object-" + Guid.NewGuid();
                string tempObjName2 = "something-" + Guid.NewGuid();

                await UploadAsync(bucket, tempObjName).ConfigureAwait(false);
                await UploadAsync(bucket, tempObjName2).ConfigureAwait(false);

                ListObjectVersionsResponse? resp = await ObjectClient.ListObjectVersionsAsync(bucket, req => req.Delimiter = "-").ConfigureAwait(false);
                Assert.True(resp.IsSuccess);

                Assert.Equal("-", resp.Delimiter);
                Assert.Equal(2, resp.CommonPrefixes!.Count);
                Assert.Equal("object-", resp.CommonPrefixes[0]);
                Assert.Equal("something-", resp.CommonPrefixes[1]);
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListObjectVersionsWithEncoding()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string tempObjName = "!#/()";

                await UploadAsync(bucket, tempObjName).ConfigureAwait(false);

                ListObjectVersionsResponse resp = await ObjectClient.ListObjectVersionsAsync(bucket, req => req.EncodingType = EncodingType.Url).ConfigureAwait(false);
                Assert.True(resp.IsSuccess);

                Assert.Equal(EncodingType.Url, resp.EncodingType);

                S3Version obj = Assert.Single(resp.Versions);

                Assert.Equal("%21%23/%28%29", obj.ObjectKey);
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

                ListObjectVersionsResponse resp = await ObjectClient.ListObjectVersionsAsync(bucket, req => req.Prefix = "object").ConfigureAwait(false);
                Assert.True(resp.IsSuccess);

                Assert.Equal("object", resp.Prefix);

                S3Version obj = Assert.Single(resp.Versions);

                Assert.Equal(tempObjName, obj.ObjectKey);
            }).ConfigureAwait(false);
        }
    }
}