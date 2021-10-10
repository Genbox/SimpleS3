using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class DeleteObjectsTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task DeleteObjects(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string BucketName = GetTestBucket(profile);

            S3DeleteInfo[] resources = new S3DeleteInfo[2];
            resources[0] = new S3DeleteInfo(nameof(DeleteObjects) + "1");
            resources[1] = new S3DeleteInfo(nameof(DeleteObjects) + "2", "versionnotfound");

            await client.PutObjectAsync(BucketName, resources[0].ObjectKey, null).ConfigureAwait(false);
            await client.PutObjectAsync(BucketName, resources[1].ObjectKey, null).ConfigureAwait(false);

            DeleteObjectsResponse resp = await client.DeleteObjectsAsync(BucketName, resources, req => req.Quiet = false).ConfigureAwait(false);

            S3DeletedObject? delObj = Assert.Single(resp.Deleted);
            Assert.Equal(resources[0].ObjectKey, delObj.ObjectKey);
            Assert.True(delObj.IsDeleteMarker);
            Assert.NotEmpty(delObj.DeleteMarkerVersionId);

            S3DeleteError? errorObj = Assert.Single(resp.Errors);

            Assert.Equal(resources[1].ObjectKey, errorObj.ObjectKey);
            Assert.Equal(resources[1].VersionId, errorObj.VersionId);
            Assert.Equal(ErrorCode.NoSuchVersion, errorObj.Code);
            Assert.Equal("The specified version does not exist.", errorObj.Message);
        }
    }
}