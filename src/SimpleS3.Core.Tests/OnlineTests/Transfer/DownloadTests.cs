using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Transfer
{
    public class DownloadTests : OnlineTestBase
    {
        public DownloadTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task DownloadContentRange()
        {
            await ObjectClient.PutObjectStringAsync(BucketName, nameof(DownloadContentRange), "123456789012345678901234567890123456789012345678901234567890").ConfigureAwait(false);

            GetObjectResponse resp = await Transfer.Download(BucketName, nameof(DownloadContentRange))
                .WithRange(0, 10)
                .ExecuteAsync()
                .ConfigureAwait(false);

            Assert.Equal(206, resp.StatusCode);
            Assert.Equal(11, resp.ContentLength);
            Assert.Equal("bytes", resp.AcceptRanges);
            Assert.Equal("bytes 0-10/60", resp.ContentRange);
            Assert.Equal("12345678901", await resp.Content.AsStringAsync().ConfigureAwait(false));
        }
    }
}