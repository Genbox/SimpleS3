using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Multipart
{
    public class ListPartsTests : OnlineTestBase
    {
        public ListPartsTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task ListParts()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                //We add the special characters at the end to test EncodingType support.
                string objName = nameof(ListParts) + "%";

                CreateMultipartUploadResponse createResp = await MultipartClient.CreateMultipartUploadAsync(bucket, objName).ConfigureAwait(false);

                ListPartsResponse listResp1 = await MultipartClient.ListPartsAsync(bucket, objName, createResp.UploadId).ConfigureAwait(false);

                Assert.Equal(bucket, listResp1.BucketName);
                Assert.Equal(objName, listResp1.ObjectKey);
                Assert.Equal(createResp.UploadId, listResp1.UploadId);
                Assert.Equal(StorageClass.Standard, listResp1.StorageClass);
                Assert.Equal(0, listResp1.PartNumberMarker);
                Assert.Equal(0, listResp1.NextPartNumberMarker);
                Assert.Equal(1000, listResp1.MaxParts);
                Assert.False(listResp1.IsTruncated);
                Assert.Equal(TestConstants.TestUsername, listResp1.Owner.Name);

                Assert.Empty(listResp1.Parts);

                UploadPartResponse uploadResp;

                byte[] file = new byte[5 * 1024];

                using (MemoryStream ms = new MemoryStream(file))
                    uploadResp = await MultipartClient.UploadPartAsync(bucket, objName, 1, createResp.UploadId, ms).ConfigureAwait(false);

                ListPartsResponse listResp2 = await MultipartClient.ListPartsAsync(bucket, objName, createResp.UploadId, req => req.EncodingType = EncodingType.Url).ConfigureAwait(false);

                Assert.Equal(bucket, listResp2.BucketName);
                Assert.Equal(WebUtility.UrlEncode(objName), listResp2.ObjectKey); //It should be encoded at this point
                Assert.Equal(createResp.UploadId, listResp2.UploadId);
                Assert.Equal(StorageClass.Standard, listResp2.StorageClass);
                Assert.Equal(0, listResp2.PartNumberMarker);
                Assert.Equal(1, listResp2.NextPartNumberMarker);
                Assert.Equal(1000, listResp2.MaxParts);
                Assert.False(listResp2.IsTruncated);
                Assert.Equal(TestConstants.TestUsername, listResp2.Owner.Name);

                S3Part part = Assert.Single(listResp2.Parts);

                Assert.Equal(1, part.PartNumber);
                Assert.Equal(DateTime.UtcNow, part.LastModified.DateTime, TimeSpan.FromSeconds(5));
                Assert.Equal("\"32ca18808933aa12e979375d07048a11\"", part.ETag);
                Assert.Equal(file.Length, part.Size);

                await MultipartClient.CompleteMultipartUploadAsync(bucket, objName, createResp.UploadId, new[] { uploadResp }).ConfigureAwait(false);

                ListPartsResponse listResp3 = await MultipartClient.ListPartsAsync(bucket, objName, createResp.UploadId).ConfigureAwait(false);
                Assert.False(listResp3.IsSuccess);
                Assert.Equal(404, listResp3.StatusCode);
            }).ConfigureAwait(false);
        }
    }
}