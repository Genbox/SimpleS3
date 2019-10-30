using System;
using System.IO;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Buckets
{
    public class ListMultipartUploadsTests : LiveTestBase
    {
        public ListMultipartUploadsTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task ListIncompleteParts()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                CreateMultipartUploadResponse initResp = await ObjectClient.CreateMultipartUploadAsync(bucket, nameof(ListIncompleteParts)).ConfigureAwait(false);

                byte[] file = new byte[5 * 1024];

                using (MemoryStream ms = new MemoryStream(file))
                    await ObjectClient.UploadPartAsync(bucket, nameof(ListIncompleteParts), 1, initResp.UploadId, ms).ConfigureAwait(false);

                ListMultipartUploadsResponse listResp = await BucketClient.ListMultipartUploadsAsync(bucket).ConfigureAwait(false);

                Assert.Equal(bucket, listResp.Bucket);
                Assert.Equal("ListIncompleteParts", listResp.NextKeyMarker);
                Assert.NotEmpty(listResp.NextUploadIdMarker);
                Assert.Equal(1000, listResp.MaxUploads);
                Assert.False(listResp.IsTruncated);

                S3Upload upload = Assert.Single(listResp.Uploads);

                Assert.Equal(listResp.NextKeyMarker, upload.ObjectKey);
                Assert.Equal(listResp.NextUploadIdMarker, upload.UploadId);
                Assert.Equal(StorageClass.Standard, upload.StorageClass);
                Assert.Equal(DateTime.UtcNow, upload.Initiated.DateTime, TimeSpan.FromSeconds(5));
            }).ConfigureAwait(false);
        }
    }
}