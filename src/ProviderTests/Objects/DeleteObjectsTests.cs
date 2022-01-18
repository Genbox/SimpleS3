using System.Threading.Tasks;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class DeleteObjectsTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
        public async Task DeleteObjects(S3Provider _, string bucket, ISimpleClient client)
        {
            S3DeleteInfo[] resources = new S3DeleteInfo[2];
            resources[0] = new S3DeleteInfo(nameof(DeleteObjects) + "1");
            resources[1] = new S3DeleteInfo(nameof(DeleteObjects) + "2", "versionnotfound");

            PutObjectResponse putResp1 = await client.PutObjectAsync(bucket, resources[0].ObjectKey, null).ConfigureAwait(false);
            Assert.Equal(200, putResp1.StatusCode);

            PutObjectResponse putResp2 = await client.PutObjectAsync(bucket, resources[1].ObjectKey, null).ConfigureAwait(false);
            Assert.Equal(200, putResp2.StatusCode);

            DeleteObjectsResponse delResp = await client.DeleteObjectsAsync(bucket, resources, r => r.Quiet = false).ConfigureAwait(false);
            Assert.Equal(200, delResp.StatusCode);

            S3DeletedObject? delObj = Assert.Single(delResp.Deleted);
            Assert.Equal(resources[0].ObjectKey, delObj.ObjectKey);
            Assert.True(delObj.IsDeleteMarker);
            Assert.NotEmpty(delObj.DeleteMarkerVersionId);

            S3DeleteError? errorObj = Assert.Single(delResp.Errors);

            Assert.Equal(resources[1].ObjectKey, errorObj.ObjectKey);
            Assert.Equal(resources[1].VersionId, errorObj.VersionId);
            Assert.Equal(ErrorCode.NoSuchVersion, errorObj.Code);
            Assert.NotEmpty(errorObj.Message);
        }
    }
}