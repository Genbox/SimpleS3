using System.Threading.Tasks;
using Genbox.SimpleS3.Core.ErrorHandling.Status;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Transfer
{
    public class MultipartTests: OnlineTestBase
    {
        public MultipartTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task UploadDownloadMultipart()
        {
            byte[] data = new byte[10 * 1024 * 1024]; //10 Mb

            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)(i % 255);

            MultipartUploadStatus resp = await Transfer.UploadData(BucketName, nameof(UploadDownloadMultipart), data)
                .ExecuteMultipartAsync()
                .ConfigureAwait(false);

            Assert.Equal(MultipartUploadStatus.Ok, resp);

            GetObjectResponse getResp = await Transfer.Download(BucketName, nameof(UploadDownloadMultipart)).ExecuteAsync().ConfigureAwait(false);
            Assert.True(getResp.IsSuccess);
            Assert.Equal(data, await getResp.Content.AsDataAsync());
        }
    }
}
