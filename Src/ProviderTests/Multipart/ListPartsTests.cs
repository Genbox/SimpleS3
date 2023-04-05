using System.Net;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Multipart;

public class ListPartsTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task ListParts(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            //We add the special characters at the end to test EncodingType support.
            string objName = nameof(ListParts) + "%";

            CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(tempBucket, objName).ConfigureAwait(false);

            ListPartsResponse listResp1 = await client.ListPartsAsync(tempBucket, objName, createResp.UploadId).ConfigureAwait(false);

            Assert.Equal(tempBucket, listResp1.BucketName);
            Assert.Equal(objName, listResp1.ObjectKey);
            Assert.Equal(createResp.UploadId, listResp1.UploadId);
            Assert.Equal(StorageClass.Standard, listResp1.StorageClass);
            Assert.Equal(0, listResp1.PartNumberMarker);
            Assert.Equal(0, listResp1.NextPartNumberMarker);
            Assert.Equal(1000, listResp1.MaxParts);
            Assert.False(listResp1.IsTruncated);

            if (provider == S3Provider.AmazonS3)
                Assert.Equal(TestConstants.TestUsername, listResp1.Owner.Name);

            Assert.Empty(listResp1.Parts);

            UploadPartResponse uploadResp;

            byte[] file = new byte[5 * 1024];

            await using (MemoryStream ms = new MemoryStream(file))
                uploadResp = await client.UploadPartAsync(tempBucket, objName, 1, createResp.UploadId, ms).ConfigureAwait(false);

            ListPartsResponse listResp2 = await client.ListPartsAsync(tempBucket, objName, createResp.UploadId, r => r.EncodingType = EncodingType.Url).ConfigureAwait(false);

            Assert.Equal(tempBucket, listResp2.BucketName);

            if (provider == S3Provider.AmazonS3)
                Assert.Equal(WebUtility.UrlEncode(objName), listResp2.ObjectKey); //It should be encoded at this point
            else
                Assert.Equal(objName, listResp2.ObjectKey); //Only amazon supports encoding apparently

            Assert.Equal(createResp.UploadId, listResp2.UploadId);
            Assert.Equal(StorageClass.Standard, listResp2.StorageClass);
            Assert.Equal(0, listResp2.PartNumberMarker);

            if (provider == S3Provider.GoogleCloudStorage)
                Assert.Equal(0, listResp2.NextPartNumberMarker);
            else
                Assert.Equal(1, listResp2.NextPartNumberMarker);

            Assert.Equal(1000, listResp2.MaxParts);
            Assert.False(listResp2.IsTruncated);

            if (provider == S3Provider.AmazonS3)
                Assert.Equal(TestConstants.TestUsername, listResp2.Owner.Name);

            S3Part part = Assert.Single(listResp2.Parts);

            Assert.Equal(1, part.PartNumber);
            Assert.Equal(DateTime.UtcNow, part.LastModified.DateTime, TimeSpan.FromSeconds(5));
            Assert.Equal("\"32ca18808933aa12e979375d07048a11\"", part.ETag);
            Assert.Equal(file.Length, part.Size);

            CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(tempBucket, objName, createResp.UploadId, new[] { uploadResp }).ConfigureAwait(false);
            Assert.Equal(200, completeResp.StatusCode);

            ListPartsResponse listResp3 = await client.ListPartsAsync(tempBucket, objName, createResp.UploadId).ConfigureAwait(false);
            Assert.Equal(404, listResp3.StatusCode);
        }).ConfigureAwait(false);
    }
}