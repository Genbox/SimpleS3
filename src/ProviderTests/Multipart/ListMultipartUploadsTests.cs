using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Multipart
{
    public class ListMultipartUploadsTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task ListMultipartUploads(S3Provider provider, IProfile profile, ISimpleClient client)
        {
            await CreateTempBucketAsync(provider, client, async bucket =>
            {
                //The percentage sign at the end is to test if encoding works correctly
                string objName = nameof(ListMultipartUploads) + "%";

                CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(bucket, objName).ConfigureAwait(false);
                Assert.Equal(200, createResp.StatusCode);

                byte[] file = new byte[5 * 1024];

                await using (MemoryStream ms = new MemoryStream(file))
                    await client.UploadPartAsync(bucket, objName, 1, createResp.UploadId, ms).ConfigureAwait(false);

                ListMultipartUploadsResponse listResp = await client.ListMultipartUploadsAsync(bucket, r => r.EncodingType = EncodingType.Url).ConfigureAwait(false);
                Assert.Equal(200, listResp.StatusCode);
                Assert.Equal(bucket, listResp.Bucket);

                if (provider == S3Provider.AmazonS3)
                {
                    Assert.Equal(WebUtility.UrlEncode(objName), listResp.NextKeyMarker);
                    Assert.NotEmpty(listResp.NextUploadIdMarker);
                }

                Assert.Equal(1000, listResp.MaxUploads);
                Assert.False(listResp.IsTruncated);

                S3Upload? upload = Assert.Single(listResp.Uploads);
                Assert.Equal(WebUtility.UrlEncode(objName), upload.ObjectKey);

                if (provider == S3Provider.AmazonS3)
                    Assert.Equal(listResp.NextUploadIdMarker, upload.UploadId);

                Assert.Equal(StorageClass.Standard, upload.StorageClass);
                Assert.Equal(DateTime.UtcNow, upload.Initiated.DateTime, TimeSpan.FromSeconds(5));
            }).ConfigureAwait(false);
        }
    }
}