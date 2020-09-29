using System.IO;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.ErrorHandling.Status;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Transfer
{
    public class MultipartTests : OnlineTestBase
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

            MultipartUploadStatus ulStatus = await Transfer.Upload(BucketName, nameof(UploadDownloadMultipart))
                                                           .UploadMultipartAsync(new MemoryStream(data))
                                                           .ConfigureAwait(false);

            Assert.Equal(MultipartUploadStatus.Ok, ulStatus);

            using (MemoryStream ms = new MemoryStream())
            {
                MultipartDownloadStatus dlStatus = await Transfer.Download(BucketName, nameof(UploadDownloadMultipart))
                                                                 .DownloadMultipartAsync(ms)
                                                                 .ConfigureAwait(false);

                Assert.Equal(MultipartDownloadStatus.Ok, dlStatus);
                Assert.Equal(data, ms.ToArray());
            }
        }
    }
}