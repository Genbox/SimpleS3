using Genbox.SimpleS3.Cli.Core.Enums;

namespace Genbox.SimpleS3.Cli.Core.Structs;

public struct PathData
{
    public PathData(string bucket, string resource, LocationType locationType, ResourceType resourceType)
    {
        Bucket = bucket;
        Resource = resource;
        LocationType = locationType;
        ResourceType = resourceType;
    }

    public string Bucket { get; }
    public string Resource { get; }
    public LocationType LocationType { get; }
    public ResourceType ResourceType { get; }
}