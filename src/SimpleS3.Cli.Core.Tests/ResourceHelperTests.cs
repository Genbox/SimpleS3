using Genbox.SimpleS3.Cli.Core.Enums;
using Genbox.SimpleS3.Cli.Core.Helpers;
using Xunit;

namespace Genbox.SimpleS3.Cli.Core.Tests
{
    public class ResourceHelperTests
    {
        [Theory]
        [InlineData("s3://bucket/resource", "bucket", "resource", LocationType.Remote, ResourceType.File)]
        [InlineData("s3://bucket/directory/", "bucket", "directory/", LocationType.Remote, ResourceType.Directory)]
        [InlineData("s3://bucket/resource*", "bucket", "resource*", LocationType.Remote, ResourceType.Expand)]
        [InlineData("s3://bucket/directory/resource", "bucket", "directory/resource", LocationType.Remote, ResourceType.File)]
        public void Parse(string resource, string expectedBucket, string expectedResource, LocationType expectedLocationType, ResourceType expectedResourceType)
        {
            Assert.True(ResourceHelper.TryParseResource(resource, out (string bucket, string resource, LocationType locationType, ResourceType resourceType) data));

            Assert.Equal(expectedBucket, data.bucket);
            Assert.Equal(expectedResource, data.resource);
            Assert.Equal(expectedLocationType, data.locationType);
            Assert.Equal(expectedResourceType, data.resourceType);
        }
    }
}