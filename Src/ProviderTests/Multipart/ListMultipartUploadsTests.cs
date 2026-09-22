using System.Net;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Multipart;

public class ListMultipartUploadsTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task SameKeyPaginationUsesBothMarkers(S3Provider provider, string bucket, ISimpleClient client)
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        string prefix = "multipart-marker-pair-" + Guid.NewGuid().ToString("N");
        string key = prefix + "/same-key";
        List<string> uploadIds = [];

        try
        {
            for (int i = 0; i < 2; i++)
            {
                CreateMultipartUploadResponse created = await client.CreateMultipartUploadAsync(bucket, key, token: token);
                Assert.True(created.IsSuccess);
                Assert.False(string.IsNullOrEmpty(created.UploadId));
                uploadIds.Add(created.UploadId);
                if (provider == S3Provider.BackBlazeB2 && i == 0)
                    await Task.Delay(TimeSpan.FromSeconds(1.1), token);
            }

            ListMultipartUploadsResponse first = await client.ListMultipartUploadsAsync(bucket, request =>
            {
                request.Prefix = prefix;
                request.MaxUploads = 1;
            }, token);
            Assert.True(first.IsSuccess);

            if (provider == S3Provider.Wasabi)
            {
                Assert.False(first.IsTruncated);
                Assert.Equal(1000, first.MaxUploads);
                Assert.Equal(key, first.NextKeyMarker);
                Assert.Empty(first.NextUploadIdMarker);
                Assert.Equal(2, first.Uploads.Count);
                Assert.All(first.Uploads, upload => Assert.Equal(key, upload.ObjectKey));
                Assert.True(uploadIds.ToHashSet(StringComparer.Ordinal).SetEquals(first.Uploads.Select(upload => upload.UploadId)));

                return;
            }

            Assert.True(first.IsTruncated);
            Assert.Equal(1, first.MaxUploads);
            S3Upload firstUpload = Assert.Single(first.Uploads);
            Assert.Equal(key, firstUpload.ObjectKey);
            Assert.Contains(firstUpload.UploadId, uploadIds);
            Assert.Equal(key, first.NextKeyMarker);
            Assert.False(string.IsNullOrEmpty(first.NextUploadIdMarker));
            Assert.Equal(firstUpload.UploadId, first.NextUploadIdMarker);

            ListMultipartUploadsResponse second = await client.ListMultipartUploadsAsync(bucket, request =>
            {
                request.Prefix = prefix;
                request.MaxUploads = 1;
                request.KeyMarker = first.NextKeyMarker;
                request.UploadIdMarker = first.NextUploadIdMarker;
            }, token);
            Assert.True(second.IsSuccess);
            Assert.False(second.IsTruncated);
            Assert.Equal(1, second.MaxUploads);
            Assert.Equal(first.NextKeyMarker, second.KeyMarker);
            Assert.Equal(first.NextUploadIdMarker, second.UploadIdMarker);

            // B2 has been observed returning either the remaining upload or an empty terminal page for this marker pair.
            if (provider == S3Provider.BackBlazeB2 && second.Uploads.Count == 0)
                return;

            S3Upload secondUpload = Assert.Single(second.Uploads);
            Assert.Equal(key, secondUpload.ObjectKey);
            Assert.Contains(secondUpload.UploadId, uploadIds);
            Assert.NotEqual(firstUpload.UploadId, secondUpload.UploadId);
            Assert.True(uploadIds.ToHashSet(StringComparer.Ordinal).SetEquals([firstUpload.UploadId, secondUpload.UploadId]));
        }
        finally
        {
            using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            foreach (string uploadId in uploadIds)
            {
                AbortMultipartUploadResponse aborted = await client.AbortMultipartUploadAsync(bucket, key, uploadId, token: cleanup.Token);
                Assert.Equal(204, aborted.StatusCode);
                Assert.Equal(404, (await client.ListPartsAsync(bucket, key, uploadId, token: cleanup.Token)).StatusCode);
            }

            ListMultipartUploadsResponse remaining = await client.ListMultipartUploadsAsync(bucket, request => request.Prefix = prefix, cleanup.Token);
            Assert.True(remaining.IsSuccess);
            Assert.DoesNotContain(remaining.Uploads, upload => upload.ObjectKey == key && uploadIds.Contains(upload.UploadId, StringComparer.Ordinal));
        }
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task ListMultipartUploads(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        //The percentage sign at the end is to test if encoding works correctly
        const string objName = nameof(ListMultipartUploads) + "%";

        CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(tempBucket, objName, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, createResp.StatusCode);
        Assert.NotNull(createResp.UploadId);

        byte[] file = new byte[5 * 1024];

        await using MemoryStream ms = new MemoryStream(file);
        await client.UploadPartAsync(tempBucket, objName, 1, createResp.UploadId, ms, token: TestContext.Current.CancellationToken);

        ListMultipartUploadsResponse listResp = await client.ListMultipartUploadsAsync(tempBucket, r => r.EncodingType = EncodingType.Url, TestContext.Current.CancellationToken);
        Assert.Equal(200, listResp.StatusCode);
        Assert.Equal(tempBucket, listResp.Bucket);

        if (provider == S3Provider.AmazonS3)
        {
            Assert.Equal(objName, listResp.NextKeyMarker);
            Assert.NotNull(listResp.NextUploadIdMarker);
            Assert.NotEmpty(listResp.NextUploadIdMarker);
        }

        Assert.Equal(1000, listResp.MaxUploads);
        Assert.False(listResp.IsTruncated);

        S3Upload upload = Assert.Single(listResp.Uploads);
        Assert.Equal(WebUtility.UrlEncode(objName), upload.ObjectKey);

        if (provider == S3Provider.AmazonS3)
            Assert.Equal(listResp.NextUploadIdMarker, upload.UploadId);

        Assert.Equal(StorageClass.Standard, upload.StorageClass);
        Assert.Equal(DateTime.UtcNow, upload.Initiated.UtcDateTime, TimeSpan.FromSeconds(60));
    });
}