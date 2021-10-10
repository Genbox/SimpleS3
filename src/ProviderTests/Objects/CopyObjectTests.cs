using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class CopyObjectTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task CopyObject(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            //Upload an object to copy
            string sourceKey = nameof(CopyObject);
            string destinationKey = sourceKey + "2";

            string bucketName = GetTestBucket(profile);

            await client.PutObjectStringAsync(bucketName, sourceKey, "test").ConfigureAwait(false);

            CopyObjectResponse copyResp = await client.CopyObjectAsync(bucketName, sourceKey, bucketName, destinationKey).ConfigureAwait(false);
            Assert.Equal(200, copyResp.StatusCode);

            GetObjectResponse getResp = await client.GetObjectAsync(bucketName, destinationKey);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal("test", await getResp.Content.AsStringAsync());
        }
    }
}