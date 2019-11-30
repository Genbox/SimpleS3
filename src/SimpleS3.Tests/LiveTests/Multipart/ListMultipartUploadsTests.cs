using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Multipart
{
    public class ListMultipartUploadsTests : LiveTestBase
    {
        public ListMultipartUploadsTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task ListMultipartUploads()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                string objName = nameof(ListMultipartUploads) + "%";

                CreateMultipartUploadResponse createResp = await MultipartClient.CreateMultipartUploadAsync(bucket, objName).ConfigureAwait(false);

                byte[] file = new byte[5 * 1024];

                using (MemoryStream ms = new MemoryStream(file))
                    await MultipartClient.UploadPartAsync(bucket, objName, 1, createResp.UploadId, ms).ConfigureAwait(false);

                ListMultipartUploadsResponse listResp = await MultipartClient.ListMultipartUploadsAsync(bucket, req => req.EncodingType = EncodingType.Url).ConfigureAwait(false);

                Assert.Equal(bucket, listResp.Bucket);
                Assert.Equal(WebUtility.UrlEncode(objName), listResp.NextKeyMarker);
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