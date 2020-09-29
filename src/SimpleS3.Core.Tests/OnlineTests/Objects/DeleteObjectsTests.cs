using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Objects
{
    public class DeleteObjectsTests : OnlineTestBase
    {
        public DeleteObjectsTests(ITestOutputHelper helper) : base(helper) { }

        [Fact]
        public async Task DeleteObjects()
        {
            List<S3DeleteInfo> resources = new List<S3DeleteInfo>(2);
            resources.Add(new S3DeleteInfo(nameof(DeleteObjects) + "1"));
            resources.Add(new S3DeleteInfo(nameof(DeleteObjects) + "2", "versionnotfound"));

            await UploadAsync(resources[0].Name).ConfigureAwait(false);
            await UploadAsync(resources[1].Name).ConfigureAwait(false);

            DeleteObjectsResponse resp = await ObjectClient.DeleteObjectsAsync(BucketName, resources, req => req.Quiet = false).ConfigureAwait(false);

            S3DeletedObject? delObj = Assert.Single(resp.Deleted);
            Assert.Equal(resources[0].Name, delObj.ObjectKey);
            Assert.True(delObj.IsDeleteMarker);
            Assert.NotEmpty(delObj.DeleteMarkerVersionId);

            S3DeleteError? errorObj = Assert.Single(resp.Errors);

            Assert.Equal(resources[1].Name, errorObj.ObjectKey);
            Assert.Equal(resources[1].VersionId, errorObj.VersionId);
            Assert.Equal(ErrorCode.NoSuchVersion, errorObj.Code);
            Assert.Equal("The specified version does not exist.", errorObj.Message);
        }

        [Fact(Skip = "Require a setup of another AWS account with 'Requester pays' setup")]
        public async Task DeleteObjectsRequestPayer()
        {
            PutObjectResponse putResp2 = await ObjectClient.PutObjectAsync(BucketName, nameof(DeleteObjectsRequestPayer), null, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(putResp2.RequestCharged);

            DeleteObjectsResponse delResp2 = await ObjectClient.DeleteObjectsAsync(BucketName, new[] { nameof(DeleteObjectsRequestPayer) }, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(delResp2.RequestCharged);
        }
    }
}