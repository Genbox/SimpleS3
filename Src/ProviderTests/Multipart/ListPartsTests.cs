using System.Net;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Multipart;

public class ListPartsTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi)]
    public async Task ListPartsRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CheckPartsAsync(provider, client, false);

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi)]
    public async Task ListPartsUrlEncoding(S3Provider provider, string _, ISimpleClient client) => await CheckPartsAsync(provider, client, true);

    private static async Task CheckPartsAsync(S3Provider provider, ISimpleClient client, bool encoded) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        //We add the special characters at the end to test EncodingType support.
        string objName = encoded ? "parts%" : "parts";

        CreateMultipartUploadResponse created = await client.CreateMultipartUploadAsync(tempBucket, objName, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, created.StatusCode);
        Assert.NotNull(created.UploadId);

        byte[] file = new byte[5 * 1024];
        UploadPartResponse uploaded = await UploadTestPartAsync(client, tempBucket, objName, created.UploadId, 1, file);
        Assert.NotNull(uploaded.ETag);

        ListPartsResponse listing = await client.ListPartsAsync(tempBucket, objName, created.UploadId, r => r.EncodingType = encoded ? EncodingType.Url : EncodingType.Unknown, TestContext.Current.CancellationToken);
        Assert.Equal(200, listing.StatusCode);

        Assert.Equal(tempBucket, listing.BucketName);

        if (provider == S3Provider.AmazonS3 && encoded)
            Assert.Equal(WebUtility.UrlEncode(objName), listing.ObjectKey); //It should be encoded at this point
        else
            Assert.Equal(objName, listing.ObjectKey); //Only amazon supports encoding apparently

        Assert.Equal(created.UploadId, listing.UploadId);
        Assert.Equal(StorageClass.Standard, listing.StorageClass);
        Assert.Equal(0, listing.PartNumberMarker);

        Assert.Equal(1, listing.NextPartNumberMarker);

        Assert.Equal(1000, listing.MaxParts);
        Assert.False(listing.IsTruncated);

        S3Part part = Assert.Single(listing.Parts);

        Assert.Equal(1, part.PartNumber);
        Assert.Equal(DateTime.UtcNow, part.LastModified.UtcDateTime, TimeSpan.FromSeconds(5));
        Assert.Equal(uploaded.ETag, part.ETag);
        Assert.Equal(file.Length, part.Size);

        if (provider == S3Provider.Wasabi)
        {
            ListBucketsResponse buckets = await client.ListBucketsAsync(token: TestContext.Current.CancellationToken);
            Assert.Equal(200, buckets.StatusCode);
            Assert.NotNull(buckets.Owner);
            Assert.NotNull(buckets.Owner.Id);
            Assert.NotNull(buckets.Owner.Name);
            Assert.NotNull(listing.Owner);
            Assert.Equal(buckets.Owner.Id, listing.Owner.Id);
            Assert.Equal(buckets.Owner.Name, listing.Owner.Name);
        }
        else
            Assert.Null(listing.Owner?.Name);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi)]
    public async Task EmptyUpload(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CreateMultipartUploadResponse created = await client.CreateMultipartUploadAsync(bucket, "empty", token: TestContext.Current.CancellationToken);
        Assert.Equal(200, created.StatusCode);
        ListPartsResponse response = await client.ListPartsAsync(bucket, "empty", created.UploadId, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        Assert.Empty(response.Parts);
        Assert.False(response.IsTruncated);
        Assert.Equal(0, response.PartNumberMarker);
        Assert.Equal(0, response.NextPartNumberMarker);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi)]
    public async Task CompletedUploadIsNotListed(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CreateMultipartUploadResponse created = await client.CreateMultipartUploadAsync(bucket, "completed", token: TestContext.Current.CancellationToken);
        Assert.Equal(200, created.StatusCode);
        UploadPartResponse part = await UploadTestPartAsync(client, bucket, "completed", created.UploadId, 1, "part"u8.ToArray());
        CompleteMultipartUploadResponse complete = await client.CompleteMultipartUploadAsync(bucket, "completed", created.UploadId, [new S3PartInfo(part.ETag, 1)], token: TestContext.Current.CancellationToken);
        Assert.Equal(200, complete.StatusCode);
        Assert.Equal(404, (await client.ListPartsAsync(bucket, "completed", created.UploadId, token: TestContext.Current.CancellationToken)).StatusCode);
    });
}