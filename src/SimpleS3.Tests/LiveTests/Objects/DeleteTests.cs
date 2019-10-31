using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class DeleteTests : LiveTestBase
    {
        public DeleteTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task DeleteMarker()
        {
            await UploadAsync(nameof(DeleteMarker)).ConfigureAwait(false);
            DeleteObjectResponse resp = await ObjectClient.DeleteObjectAsync(BucketName, nameof(DeleteMarker)).ConfigureAwait(false);

            Assert.True(resp.IsDeleteMarker);
        }

        [Fact]
        public async Task DeleteMultiple()
        {
            List<S3DeleteInfo> resources = new List<S3DeleteInfo>(2);
            resources.Add(new S3DeleteInfo(nameof(DeleteMultiple) + "1"));
            resources.Add(new S3DeleteInfo(nameof(DeleteMultiple) + "2", "versionnotfound"));

            await UploadAsync(resources[0].Name).ConfigureAwait(false);
            await UploadAsync(resources[1].Name).ConfigureAwait(false);

            DeleteObjectsResponse resp = await ObjectClient.DeleteObjectsAsync(BucketName, resources, req => req.Quiet = false).ConfigureAwait(false);

            S3DeletedObject delObj = Assert.Single(resp.Deleted);
            Assert.Equal(resources[0].Name, delObj.ObjectKey);
            Assert.True(delObj.IsDeleteMarker);
            Assert.NotEmpty(delObj.DeleteMarkerVersionId);

            S3DeleteError errorObj = Assert.Single(resp.Errors);

            Assert.Equal(resources[1].Name, errorObj.ObjectKey);
            Assert.Equal(resources[1].VersionId, errorObj.VersionId);
            Assert.Equal(ErrorCode.NoSuchVersion, errorObj.Code);
            Assert.Equal("The specified version does not exist.", errorObj.Message);
        }
    }
}