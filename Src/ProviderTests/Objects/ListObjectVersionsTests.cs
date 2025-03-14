﻿using System.Globalization;
using System.Security.Cryptography;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class ListObjectVersionsTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)] //This test returns the wrong IsLatest on Google
    public async Task ListObjectVersions(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            //Enable versioning on the bucket
            await client.PutBucketVersioningAsync(tempBucket, true);

            //Verify that we enabled bucket versioning
            GetBucketVersioningResponse getVerResp = await client.GetBucketVersioningAsync(tempBucket);
            Assert.True(getVerResp.Status);

            PutObjectResponse putResp1 = await client.PutObjectStringAsync(tempBucket, "1", "a");
            PutObjectResponse putResp2 = await client.PutObjectStringAsync(tempBucket, "2", "aa");
            PutObjectResponse putResp3 = await client.PutObjectStringAsync(tempBucket, "3", "aaa");

            DeleteObjectResponse putResp4 = await client.DeleteObjectAsync(tempBucket, "2"); //Delete object 2
            PutObjectResponse putResp5 = await client.PutObjectStringAsync(tempBucket, "3", "aaaa"); //Overwrite object 3

            ListObjectVersionsResponse listResp = await client.ListObjectVersionsAsync(tempBucket);
            Assert.True(listResp.IsSuccess);
            Assert.Equal(4, listResp.Versions.Count);

            if (provider == S3Provider.AmazonS3)
            {
                Assert.Single(listResp.DeleteMarkers);
                Assert.Equal(1000, listResp.MaxKeys);
            }

            Assert.Equal(tempBucket, listResp.BucketName);
            Assert.False(listResp.IsTruncated);

            S3Version version1 = listResp.Versions[0];
            Assert.Equal("1", version1.ObjectKey);
            Assert.Equal(putResp1.VersionId, version1.VersionId);
            Assert.True(version1.IsLatest);
            Assert.Equal(DateTimeOffset.UtcNow.DateTime, version1.LastModified.DateTime, TimeSpan.FromMinutes(1));
            Assert.Equal("\"0cc175b9c0f1b6a831c399e269772661\"", version1.Etag);
            Assert.Equal(1, version1.Size);

            if (provider != S3Provider.GoogleCloudStorage)
                Assert.Equal(StorageClass.Standard, version1.StorageClass);

            if (provider == S3Provider.AmazonS3)
            {
                Assert.Equal(TestConstants.TestUserId, version1.Owner?.Id);
                Assert.Equal(TestConstants.TestUsername, version1.Owner?.Name);
            }

            S3Version version2 = listResp.Versions[1];
            Assert.Equal("2", version2.ObjectKey);
            Assert.Equal(putResp2.VersionId, version2.VersionId);
            Assert.False(version2.IsLatest);
            Assert.Equal(DateTimeOffset.UtcNow.DateTime, version2.LastModified.DateTime, TimeSpan.FromMinutes(1));
            Assert.Equal("\"4124bc0a9335c27f086f24ba207a4912\"", version2.Etag);
            Assert.Equal(2, version2.Size);

            if (provider != S3Provider.GoogleCloudStorage)
                Assert.Equal(StorageClass.Standard, version2.StorageClass);

            if (provider == S3Provider.AmazonS3)
            {
                Assert.Equal(TestConstants.TestUserId, version2.Owner?.Id);
                Assert.Equal(TestConstants.TestUsername, version2.Owner?.Name);
            }

            //This is the latest version of object 3 and should be 4 in size
            S3Version version3 = listResp.Versions[2];
            Assert.Equal("3", version3.ObjectKey);
            Assert.Equal(putResp5.VersionId, version3.VersionId);
            Assert.True(version3.IsLatest);
            Assert.Equal(DateTimeOffset.UtcNow.DateTime, version3.LastModified.DateTime, TimeSpan.FromMinutes(1));
            Assert.Equal("\"74b87337454200d4d33f80c4663dc5e5\"", version3.Etag);
            Assert.Equal(4, version3.Size);

            if (provider != S3Provider.GoogleCloudStorage)
                Assert.Equal(StorageClass.Standard, version3.StorageClass);

            if (provider == S3Provider.AmazonS3)
            {
                Assert.Equal(TestConstants.TestUserId, version3.Owner?.Id);
                Assert.Equal(TestConstants.TestUsername, version3.Owner?.Name);
            }

            //This was the previous version of object 3, so it should not be the latest and have 3 in size
            S3Version version3A = listResp.Versions[3];
            Assert.Equal("3", version3A.ObjectKey);
            Assert.Equal(putResp3.VersionId, version3A.VersionId);
            Assert.False(version3A.IsLatest);
            Assert.Equal(DateTimeOffset.UtcNow.DateTime, version3A.LastModified.DateTime, TimeSpan.FromMinutes(1));
            Assert.Equal("\"47bce5c74f589f4867dbd57e9ca9f808\"", version3A.Etag);
            Assert.Equal(3, version3A.Size);

            if (provider != S3Provider.GoogleCloudStorage)
                Assert.Equal(StorageClass.Standard, version3A.StorageClass);

            if (provider == S3Provider.AmazonS3)
            {
                Assert.Equal(TestConstants.TestUserId, version3A.Owner?.Id);
                Assert.Equal(TestConstants.TestUsername, version3A.Owner?.Name);
            }

            //This is the latest version of object 2, since it was deleted
            S3DeleteMarker delMarker = listResp.DeleteMarkers[0];
            Assert.True(delMarker.IsLatest);
            Assert.Equal("2", delMarker.ObjectKey);
            Assert.Equal(putResp4.VersionId, delMarker.VersionId);
            Assert.Equal(DateTimeOffset.UtcNow.DateTime, delMarker.LastModified.DateTime, TimeSpan.FromMinutes(1));

            if (provider == S3Provider.AmazonS3)
            {
                Assert.Equal(TestConstants.TestUserId, delMarker.Owner.Id);
                Assert.Equal(TestConstants.TestUsername, delMarker.Owner.Name);
            }
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task ListObjectsMoreThanMaxKeys(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            const int concurrent = 10;
            const int count = 11;

            await ParallelHelper.ExecuteAsync(Enumerable.Range(0, count), (val, token) => client.PutObjectAsync(tempBucket, val.ToString(NumberFormatInfo.InvariantInfo), null, null, token), concurrent);

            ListObjectVersionsResponse listResp = await client.ListObjectVersionsAsync(tempBucket, r => r.MaxKeys = count - 1);
            Assert.True(listResp.IsSuccess);
            Assert.True(listResp.IsTruncated);
            Assert.Equal(10, listResp.MaxKeys);
            Assert.Equal(10, listResp.Versions.Count);

            ListObjectVersionsResponse listResp2 = await client.ListObjectVersionsAsync(tempBucket, r => r.KeyMarker = listResp.NextKeyMarker);
            Assert.True(listResp2.IsSuccess);
            Assert.False(listResp2.IsTruncated);

            if (provider != S3Provider.GoogleCloudStorage)
                Assert.Single(listResp2.Versions);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task ListObjectVersionsWithDelimiter(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            string tempObjName = "object-" + Guid.NewGuid();
            string tempObjName2 = "something-" + Guid.NewGuid();

            await client.PutObjectAsync(tempBucket, tempObjName, null);
            await client.PutObjectAsync(tempBucket, tempObjName2, null);

            ListObjectVersionsResponse resp = await client.ListObjectVersionsAsync(tempBucket, r => r.Delimiter = "-");
            Assert.True(resp.IsSuccess);

            Assert.Equal("-", resp.Delimiter);
            Assert.Equal(2, resp.CommonPrefixes.Count);
            Assert.Equal("object-", resp.CommonPrefixes[0]);
            Assert.Equal("something-", resp.CommonPrefixes[1]);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task ListObjectVersionsWithEncoding(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            const string tempObjName = "!#/()";

            await client.PutObjectAsync(tempBucket, tempObjName, null);

            ListObjectVersionsResponse resp = await client.ListObjectVersionsAsync(tempBucket, r => r.EncodingType = EncodingType.Url);
            Assert.True(resp.IsSuccess);

            Assert.Equal(EncodingType.Url, resp.EncodingType);

            S3Version obj = Assert.Single(resp.Versions);

            Assert.Equal("%21%23/%28%29", obj.ObjectKey);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task ListObjectsWithPrefix(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            string tempObjName = "object-" + Guid.NewGuid();
            string tempObjName2 = "something-" + Guid.NewGuid();

            await client.PutObjectAsync(tempBucket, tempObjName, null);
            await client.PutObjectAsync(tempBucket, tempObjName2, null);

            ListObjectVersionsResponse resp = await client.ListObjectVersionsAsync(tempBucket, r => r.Prefix = "object");
            Assert.True(resp.IsSuccess);

            Assert.Equal("object", resp.Prefix);

            S3Version obj = Assert.Single(resp.Versions);

            Assert.Equal(tempObjName, obj.ObjectKey);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task ListObjectsChecksum(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            string tempObjName = "object-" + Guid.NewGuid();
            byte[] data = "hello world"u8.ToArray();
            byte[] checksum = SHA1.HashData(data);

            PutObjectResponse putResp = await client.PutObjectDataAsync(tempBucket, tempObjName, data, r =>
            {
                r.ChecksumAlgorithm = ChecksumAlgorithm.Sha1;
                r.Checksum = checksum;
            });

            Assert.Equal(200, putResp.StatusCode);

            ListObjectVersionsResponse listResp = await client.ListObjectVersionsAsync(tempBucket);
            Assert.Equal(200, listResp.StatusCode);
            S3Version obj = Assert.Single(listResp.Versions);

            Assert.Equal(ChecksumAlgorithm.Sha1, obj.ChecksumAlgorithm);
            Assert.Equal(ChecksumType.FullObject, obj.ChecksumType);
        });
    }
}