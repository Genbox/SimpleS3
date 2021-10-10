using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class ObjectLegalHoldTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutGetObjectLegalHold(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string objectKey = nameof(PutGetObjectLegalHold);
            string bucketName = GetTestBucket(profile);

            //Create an object
            await client.PutObjectAsync(bucketName, objectKey, null).ConfigureAwait(false);

            //Check that there is no lock
            GetObjectLegalHoldResponse getLegalResp = await client.GetObjectLegalHoldAsync(bucketName, objectKey).ConfigureAwait(false);
            Assert.Equal(404, getLegalResp.StatusCode);
            Assert.False(getLegalResp.LegalHold);

            //Set a lock
            PutObjectLegalHoldResponse putLegalResp = await client.PutObjectLegalHoldAsync(bucketName, objectKey, true).ConfigureAwait(false);
            Assert.Equal(200, putLegalResp.StatusCode);

            //There should be a lock now
            GetObjectLegalHoldResponse getLegalResp2 = await client.GetObjectLegalHoldAsync(bucketName, objectKey).ConfigureAwait(false);
            Assert.Equal(200, getLegalResp2.StatusCode);
            Assert.True(getLegalResp2.LegalHold);
        }
    }
}