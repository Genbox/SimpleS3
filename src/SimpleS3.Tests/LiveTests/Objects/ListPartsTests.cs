using System;
using System.IO;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Responses.Objects;
using Genbox.SimpleS3.Core.Responses.S3Types;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class ListPartsTests : LiveTestBase
    {
        public ListPartsTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task SimpleList()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                CreateMultipartUploadResponse initResp = await ObjectClient.CreateMultipartUploadAsync(bucket, nameof(SimpleList)).ConfigureAwait(false);

                ListPartsResponse listResp1 = await ObjectClient.ListPartsAsync(bucket, nameof(SimpleList), initResp.UploadId).ConfigureAwait(false);

                Assert.Equal(bucket, listResp1.Bucket);
                Assert.Equal("SimpleList", listResp1.Key);
                Assert.Equal(initResp.UploadId, listResp1.UploadId);
                Assert.Equal(StorageClass.Standard, listResp1.StorageClass);
                Assert.Equal(0, listResp1.PartNumberMarker);
                Assert.Equal(0, listResp1.NextPartNumberMarker);
                Assert.Equal(1000, listResp1.MaxParts);
                Assert.False(listResp1.IsTruncated);
                Assert.Equal(TestConstants.TestUsername, listResp1.Initiator.Name);
                Assert.Equal(TestConstants.TestUsername, listResp1.Owner.Name);

                Assert.Empty(listResp1.Parts);

                UploadPartResponse uploadResp;

                byte[] file = new byte[5 * 1024];

                using (MemoryStream ms = new MemoryStream(file))
                    uploadResp = await ObjectClient.UploadPartAsync(bucket, nameof(SimpleList), 1, initResp.UploadId, ms).ConfigureAwait(false);

                ListPartsResponse listResp2 = await ObjectClient.ListPartsAsync(bucket, nameof(SimpleList), initResp.UploadId).ConfigureAwait(false);

                Assert.Equal(bucket, listResp2.Bucket);
                Assert.Equal("SimpleList", listResp2.Key);
                Assert.Equal(initResp.UploadId, listResp2.UploadId);
                Assert.Equal(StorageClass.Standard, listResp2.StorageClass);
                Assert.Equal(0, listResp2.PartNumberMarker);
                Assert.Equal(1, listResp2.NextPartNumberMarker);
                Assert.Equal(1000, listResp2.MaxParts);
                Assert.False(listResp2.IsTruncated);
                Assert.Equal(TestConstants.TestUsername, listResp2.Initiator.Name);
                Assert.Equal(TestConstants.TestUsername, listResp2.Owner.Name);

                S3Part part = Assert.Single(listResp2.Parts);

                Assert.Equal(1, part.PartNumber);
                Assert.Equal(DateTime.UtcNow, part.LastModified.DateTime, TimeSpan.FromSeconds(5));
                Assert.Equal("\"32ca18808933aa12e979375d07048a11\"", part.ETag);
                Assert.Equal(file.Length, part.Size);

                await ObjectClient.CompleteMultipartUploadAsync(bucket, nameof(SimpleList), initResp.UploadId, new[] {uploadResp}).ConfigureAwait(false);

                ListPartsResponse listResp3 = await ObjectClient.ListPartsAsync(bucket, nameof(SimpleList), initResp.UploadId).ConfigureAwait(false);
                Assert.False(listResp3.IsSuccess);
                Assert.Equal(404, listResp3.StatusCode);
            }).ConfigureAwait(false);
        }
    }
}