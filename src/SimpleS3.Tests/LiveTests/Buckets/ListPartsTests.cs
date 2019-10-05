using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.Objects;
using Genbox.SimpleS3.Core.Responses.S3Types;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Buckets
{
    public class ListPartsTests : LiveTestBase
    {
        public ListPartsTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task ListIncompleteParts()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                InitiateMultipartUploadResponse initResp = await ObjectClient.InitiateMultipartUploadAsync(bucket, nameof(ListIncompleteParts)).ConfigureAwait(false);

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

                Assert.Equal(listResp.NextKeyMarker, upload.Name);
                Assert.Equal(listResp.NextUploadIdMarker, upload.UploadId);
                Assert.Equal(TestConstants.TestUsername, upload.Initiator.Name);
                Assert.Equal(StorageClass.Standard, upload.StorageClass);
                Assert.Equal(DateTime.UtcNow, upload.Initiated.DateTime, TimeSpan.FromSeconds(5));
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task ListObjects()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                //Create 3 objects in bucket, including an incomplete multipart upload
                await UploadAsync(bucket, "resource1").ConfigureAwait(false);
                await UploadAsync(bucket, "resource2").ConfigureAwait(false);
                InitiateMultipartUploadResponse initResp = await ObjectClient.InitiateMultipartUploadAsync(bucket, "multipart").ConfigureAwait(false);

                //List the objects
                List<S3Object> list = await BucketClient.GetBucketRecursiveAsync(bucket, true).ToListAsync().ConfigureAwait(false);

                //Only 2 objects should be present, as one of them is only initiated
                Assert.Equal(2, list.Count);
                Assert.Equal("resource1", list[0].Name);
                Assert.Equal("resource2", list[1].Name);

                //List multipart transfers
                List<S3Upload> uploads = await BucketClient.ListAllMultipartUploadsAsync(bucket).ToListAsync().ConfigureAwait(false);

                S3Upload upload = Assert.Single(uploads);

                Assert.Equal("multipart", upload.Name);
            }).ConfigureAwait(false);
        }
    }
}