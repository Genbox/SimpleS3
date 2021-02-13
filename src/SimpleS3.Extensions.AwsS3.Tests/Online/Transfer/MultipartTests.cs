using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AwsS3.Tests.Online.Transfer
{
    public class MultipartTests : AwsTestBase
    {
        public MultipartTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task UploadDownloadMultipart()
        {
            byte[] data = new byte[10 * 1024 * 1024]; //10 Mb

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(i % 255);
            }

            CompleteMultipartUploadResponse uploadResp = await Transfer.CreateUpload(BucketName, nameof(UploadDownloadMultipart))
                                                                       .UploadMultipartAsync(new MemoryStream(data))
                                                                       .ConfigureAwait(false);

            Assert.True(uploadResp.IsSuccess);

            using (MemoryStream ms = new MemoryStream())
            {
                IAsyncEnumerable<GetObjectResponse>? responses = Transfer.CreateDownload(BucketName, nameof(UploadDownloadMultipart))
                                                                         .DownloadMultipartAsync(ms);

                await foreach (var resp in responses)
                {
                    Assert.True(resp.IsSuccess);
                }

                Assert.Equal(data, ms.ToArray());
            }
        }
    }
}