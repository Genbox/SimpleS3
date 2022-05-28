using Genbox.SimpleS3.Cli.Core.Enums;
using Genbox.SimpleS3.Cli.Core.Helpers;
using Genbox.SimpleS3.Cli.Core.Structs;
using Xunit;

namespace Genbox.SimpleS3.Cli.Core.Tests;

public class ResourceHelperTests
{
    [Theory]

    //Full remote paths
    [InlineData("s3://bucket/directory/resource", "bucket", "directory/resource", LocationType.Remote, ResourceType.File)]
    [InlineData("s3://bucket/directory/", "bucket", "directory/", LocationType.Remote, ResourceType.Directory)]

    //Full local paths
    [InlineData("C:\\Windows\\explorer.exe", "", "C:\\Windows\\explorer.exe", LocationType.Local, ResourceType.File)]
    [InlineData("C:\\Windows\\", "", "C:\\Windows\\", LocationType.Local, ResourceType.Directory)]

    //Relative local paths
    [InlineData("explorer.exe", "", "C:\\Windows\\explorer.exe", LocationType.Local, ResourceType.File)]
    [InlineData("", "", "C:\\Windows\\", LocationType.Local, ResourceType.Directory)]
    public void Parse(string resource, string expectedBucket, string expectedResource, LocationType expectedLocationType, ResourceType expectedResourceType)
    {
        Assert.True(ResourceHelper.TryParsePath(resource, out PathData data, "C:\\Windows\\"));

        Assert.Equal(expectedBucket, data.Bucket);
        Assert.Equal(expectedResource, data.Resource);
        Assert.Equal(expectedLocationType, data.LocationType);
        Assert.Equal(expectedResourceType, data.ResourceType);
    }
}