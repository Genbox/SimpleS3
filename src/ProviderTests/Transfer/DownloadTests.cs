using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Transfer
{
    public class DownloadTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task DownloadContentRange(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectStringAsync(bucketName, nameof(DownloadContentRange), "123456789012345678901234567890123456789012345678901234567890").ConfigureAwait(false);

            GetObjectResponse resp = await client.CreateDownload(bucketName, nameof(DownloadContentRange))
                                                 .WithRange(0, 10)
                                                 .DownloadAsync()
                                                 .ConfigureAwait(false);

            Assert.Equal(206, resp.StatusCode);
            Assert.Equal(11, resp.ContentLength);
            Assert.Equal("bytes", resp.AcceptRanges);
            Assert.Equal("bytes 0-10/60", resp.ContentRange);
            Assert.Equal("12345678901", await resp.Content.AsStringAsync().ConfigureAwait(false));
        }
    }
}