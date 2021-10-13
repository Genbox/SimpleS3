using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class DeleteObjectTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task DeleteObject(S3Provider provider, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);

            PutObjectResponse putResp = await client.PutObjectAsync(bucketName, nameof(DeleteObject), null).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            DeleteObjectResponse delREsp = await client.DeleteObjectAsync(bucketName, nameof(DeleteObject)).ConfigureAwait(false);
            Assert.Equal(204, delREsp.StatusCode);

            if (provider == S3Provider.AmazonS3)
                Assert.True(delREsp.IsDeleteMarker);
        }
    }
}