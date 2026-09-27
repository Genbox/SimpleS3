using System.Globalization;
using Genbox.ProviderTests.Code;
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
    public async Task OverwriteAndDeletePreserveVersionHistory(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        //Enable versioning on the bucket
        Assert.Equal(200, (await client.PutBucketVersioningAsync(tempBucket, true, token: TestContext.Current.CancellationToken)).StatusCode);

        //Verify that we enabled bucket versioning
        GetBucketVersioningResponse versioning = await client.GetBucketVersioningAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, versioning.StatusCode);
        Assert.True(versioning.Status);

        PutObjectResponse first = await client.PutObjectStringAsync(tempBucket, "1", "a", token: TestContext.Current.CancellationToken);
        PutObjectResponse second = await client.PutObjectStringAsync(tempBucket, "2", "aa", token: TestContext.Current.CancellationToken);
        PutObjectResponse originalThird = await client.PutObjectStringAsync(tempBucket, "3", "aaa", token: TestContext.Current.CancellationToken);

        DeleteObjectResponse deletedSecond = await client.DeleteObjectAsync(tempBucket, "2", token: TestContext.Current.CancellationToken);
        PutObjectResponse replacementThird = await client.PutObjectStringAsync(tempBucket, "3", "aaaa", token: TestContext.Current.CancellationToken);
        Assert.Equal(200, first.StatusCode);
        Assert.Equal(200, second.StatusCode);
        Assert.Equal(200, originalThird.StatusCode);
        Assert.Equal(204, deletedSecond.StatusCode);
        Assert.Equal(200, replacementThird.StatusCode);

        ListObjectVersionsResponse listing = await client.ListObjectVersionsAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.True(listing.IsSuccess);
        Assert.Equal(4, listing.Versions.Count);

        Assert.Equal(tempBucket, listing.BucketName);
        Assert.False(listing.IsTruncated);

        (string Key, string? VersionId, int Size, string? ETag, bool IsLatest)[] expected =
        [
            ("1", first.VersionId, 1, first.ETag, true),
            ("2", second.VersionId, 2, second.ETag, false),
            ("3", replacementThird.VersionId, 4, replacementThird.ETag, true),
            ("3", originalThird.VersionId, 3, originalThird.ETag, false)
        ];
        DateTime now = DateTime.UtcNow;

        for (int i = 0; i < expected.Length; i++)
        {
            S3Version version = listing.Versions[i];
            Assert.Equal(expected[i].Key, version.ObjectKey);
            Assert.Equal(expected[i].VersionId, version.VersionId);
            Assert.Equal(expected[i].Size, version.Size);
            Assert.NotNull(version.Etag);
            Assert.Equal(expected[i].ETag, version.Etag);
            Assert.Equal(expected[i].IsLatest, version.IsLatest);
            Assert.Equal(now, version.LastModified.UtcDateTime, TimeSpan.FromMinutes(1));
            Assert.Equal(StorageClass.Standard, version.StorageClass);
            Assert.NotNull(version.Owner?.Id);
            Assert.Null(version.Owner?.Name);
        }

        //This is the latest version of object 2, since it was deleted
        S3DeleteMarker deleteMarker = Assert.Single(listing.DeleteMarkers);
        Assert.True(deleteMarker.IsLatest);
        Assert.Equal("2", deleteMarker.ObjectKey);
        Assert.Equal(deletedSecond.VersionId, deleteMarker.VersionId);
        Assert.Equal(DateTimeOffset.UtcNow.UtcDateTime, deleteMarker.LastModified.UtcDateTime, TimeSpan.FromMinutes(1));

        Assert.NotNull(deleteMarker.Owner.Id);
        Assert.Null(deleteMarker.Owner.Name);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task ListObjectVersionsRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        Assert.Equal(200, (await client.PutBucketVersioningAsync(bucket, true, token: TestContext.Current.CancellationToken)).StatusCode);
        PutObjectResponse put = await client.PutObjectStringAsync(bucket, "version", "content", token: TestContext.Current.CancellationToken);
        Assert.Equal(200, put.StatusCode);
        ListObjectVersionsResponse response = await client.ListObjectVersionsAsync(bucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        S3Version version = Assert.Single(response.Versions);
        Assert.Equal("version", version.ObjectKey);
        Assert.Equal(put.VersionId, version.VersionId);
        Assert.Equal(put.ETag, version.Etag);
        Assert.Equal(7, version.Size);
        Assert.True(version.IsLatest);
        Assert.Empty(response.DeleteMarkers);
    });

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task ListObjectVersionsMoreThanMaxKeys(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        const int concurrent = 10;
        const int count = 11;

        IEnumerable<PutObjectResponse> uploads = await ParallelHelper.ExecuteAsync(Enumerable.Range(0, count), (val, token) => client.PutObjectAsync(tempBucket, val.ToString(NumberFormatInfo.InvariantInfo), null, null, token), concurrent, TestContext.Current.CancellationToken);
        Assert.All(uploads, response => Assert.Equal(200, response.StatusCode));

        ListObjectVersionsResponse listResp = await client.ListObjectVersionsAsync(tempBucket, r => r.MaxKeys = count - 1, TestContext.Current.CancellationToken);
        Assert.True(listResp.IsSuccess);
        Assert.True(listResp.IsTruncated);
        Assert.Equal(10, listResp.MaxKeys);
        Assert.Equal(10, listResp.Versions.Count);

        S3Version last = listResp.Versions[^1];
        Assert.Equal(last.ObjectKey, listResp.NextKeyMarker);
        Assert.False(string.IsNullOrEmpty(last.VersionId));
        Assert.Equal(last.VersionId, listResp.NextVersionIdMarker);

        ListObjectVersionsResponse listResp2 = await client.ListObjectVersionsAsync(tempBucket, r =>
        {
            r.KeyMarker = listResp.NextKeyMarker;
            r.VersionIdMarker = listResp.NextVersionIdMarker;
        }, TestContext.Current.CancellationToken);
        Assert.True(listResp2.IsSuccess);
        Assert.False(listResp2.IsTruncated);
        Assert.Single(listResp2.Versions);
        Assert.Equal(Enumerable.Range(0, count).Select(value => value.ToString(NumberFormatInfo.InvariantInfo)).Order(StringComparer.Ordinal),
            listResp.Versions.Concat(listResp2.Versions).Select(item => item.ObjectKey).Order(StringComparer.Ordinal));
    });

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task ListObjectVersionsWithDelimiter(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        string tempObjName = "object-" + Guid.NewGuid();
        string tempObjName2 = "something-" + Guid.NewGuid();

        Assert.Equal(200, (await client.PutObjectAsync(tempBucket, tempObjName, null, token: TestContext.Current.CancellationToken)).StatusCode);
        Assert.Equal(200, (await client.PutObjectAsync(tempBucket, tempObjName2, null, token: TestContext.Current.CancellationToken)).StatusCode);

        ListObjectVersionsResponse resp = await client.ListObjectVersionsAsync(tempBucket, r => r.Delimiter = "-", TestContext.Current.CancellationToken);
        Assert.True(resp.IsSuccess);

        Assert.Equal("-", resp.Delimiter);
        Assert.Equal(2, resp.CommonPrefixes.Count);
        Assert.Equal("object-", resp.CommonPrefixes[0]);
        Assert.Equal("something-", resp.CommonPrefixes[1]);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task ListObjectVersionsWithEncoding(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        const string tempObjName = "!#/()";

        Assert.Equal(200, (await client.PutObjectAsync(tempBucket, tempObjName, null, token: TestContext.Current.CancellationToken)).StatusCode);

        ListObjectVersionsResponse resp = await client.ListObjectVersionsAsync(tempBucket, r => r.EncodingType = EncodingType.Url, TestContext.Current.CancellationToken);
        Assert.True(resp.IsSuccess);

        Assert.Equal(EncodingType.Url, resp.EncodingType);

        S3Version obj = Assert.Single(resp.Versions);

        Assert.Equal("%21%23/%28%29", obj.ObjectKey);
    });

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task ListObjectVersionsWithPrefix(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        string tempObjName = "object-" + Guid.NewGuid();
        string tempObjName2 = "something-" + Guid.NewGuid();

        Assert.Equal(200, (await client.PutObjectAsync(tempBucket, tempObjName, null, token: TestContext.Current.CancellationToken)).StatusCode);
        Assert.Equal(200, (await client.PutObjectAsync(tempBucket, tempObjName2, null, token: TestContext.Current.CancellationToken)).StatusCode);

        ListObjectVersionsResponse resp = await client.ListObjectVersionsAsync(tempBucket, r => r.Prefix = "object", TestContext.Current.CancellationToken);
        Assert.True(resp.IsSuccess);

        Assert.Equal("object", resp.Prefix);

        S3Version obj = Assert.Single(resp.Versions);

        Assert.Equal(tempObjName, obj.ObjectKey);
    });
}