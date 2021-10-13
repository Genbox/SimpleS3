#if COMMERCIAL
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Transfer
{
    public class MultipartTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task UploadDownloadMultipart(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            byte[] data = new byte[10 * 1024 * 1024]; //10 Mb

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(i % 255);
            }

            CompleteMultipartUploadResponse uploadResp = await client.CreateUpload(bucketName, nameof(UploadDownloadMultipart))
                                                                     .UploadMultipartAsync(new MemoryStream(data))
                                                                     .ConfigureAwait(false);

            Assert.True(uploadResp.IsSuccess);

            using (MemoryStream ms = new MemoryStream())
            {
                IAsyncEnumerable<GetObjectResponse> responses = client.CreateDownload(bucketName, nameof(UploadDownloadMultipart))
                                                                      .DownloadMultipartAsync(ms);

                await foreach (GetObjectResponse resp in responses)
                {
                    Assert.True(resp.IsSuccess);
                }

                Assert.Equal(data, ms.ToArray());
            }
        }
    }
}
#endif