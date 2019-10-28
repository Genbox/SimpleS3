using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Requests.Objects.Types;
using Genbox.SimpleS3.Core.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class DeleteMarkerTests : LiveTestBase
    {
        public DeleteMarkerTests(ITestOutputHelper helper) : base(helper)
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
            List<string> resources = new List<string>(2);
            resources.Add(nameof(DeleteMultiple) + "1");
            resources.Add(nameof(DeleteMultiple) + "2");

            await UploadAsync(resources[0]).ConfigureAwait(false);
            await UploadAsync(resources[1]).ConfigureAwait(false);

            DeleteObjectsResponse resp = await ObjectClient.DeleteObjectsAsync(BucketName, resources, request => request.Quiet = false).ConfigureAwait(false);

            Assert.Equal(2, resp.Deleted.Count);
            Assert.Contains(resp.Deleted, o => o.Key == resources[0]);
            Assert.Contains(resp.Deleted, o => o.Key == resources[1]);

            Assert.All(resp.Deleted, o => Assert.True(o.DeleteMarker));
            Assert.All(resp.Deleted, o => Assert.NotEmpty(o.DeleteMarkerVersionId));
        }

        [Fact]
        public async Task DeleteMultipleWithError()
        {
            List<S3DeleteInfo> resources = new List<S3DeleteInfo>(2);
            resources.Add(new S3DeleteInfo(nameof(DeleteMultipleWithError) + "1", "versionnotfound"));
            resources.Add(new S3DeleteInfo(nameof(DeleteMultipleWithError) + "2", null));

            await UploadAsync(resources[0].Name).ConfigureAwait(false);
            await UploadAsync(resources[1].Name).ConfigureAwait(false);

            DeleteObjectsResponse resp = await ObjectClient.DeleteObjectsAsync(BucketName, resources, request => request.Quiet = false).ConfigureAwait(false);

            Assert.Equal(1, resp.Deleted.Count);
            Assert.Equal(resources[1].Name, resp.Deleted[0].Key);
            Assert.True(resp.Deleted[0].DeleteMarker);
            Assert.NotEmpty(resp.Deleted[0].DeleteMarkerVersionId);

            Assert.Equal(1, resp.Errors.Count);
            Assert.Equal(resources[0].Name, resp.Errors[0].Key);
            Assert.Equal(resources[0].VersionId, resp.Errors[0].VersionId);
            Assert.Equal("NoSuchVersion", resp.Errors[0].Code);
            Assert.Equal("The specified version does not exist.", resp.Errors[0].Message);
        }
    }
}