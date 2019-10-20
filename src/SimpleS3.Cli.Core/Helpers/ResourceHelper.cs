using System;
using Genbox.SimpleS3.Cli.Core.Enums;
using Genbox.SimpleS3.Utils.Extensions;

namespace Genbox.SimpleS3.Cli.Core.Helpers
{
    public static class ResourceHelper
    {
        public static bool TryParseResource(in string resource, out (string bucket, string resource, LocationType locationType, ResourceType resourceType) data)
        {
            if (string.IsNullOrEmpty(resource))
            {
                data = default;
                return false;
            }

            LocationType locationType = resource.StartsWith("s3://", StringComparison.OrdinalIgnoreCase) ? LocationType.Remote : LocationType.Local;
            ResourceType resourceType;

            if (locationType == LocationType.Local)
            {
                if (resource.Contains('*'))
                    resourceType = ResourceType.Expand;
                else if (resource.EndsWith('/'))
                    resourceType = ResourceType.Directory;
                else
                    resourceType = ResourceType.File;

                data = (null, resource, locationType, resourceType);
            }
            else
            {
                int indexOfSlash = resource.IndexOf('/', 5);

                string parsedBucket = resource.Substring(5, indexOfSlash - 5);
                string parsedResource = resource.Substring(indexOfSlash + 1);

                if (parsedResource.Contains('*'))
                    resourceType = ResourceType.Expand;
                else if (parsedResource.EndsWith('/'))
                    resourceType = ResourceType.Directory;
                else
                    resourceType = ResourceType.File;

                data = (parsedBucket, parsedResource, locationType, resourceType);
            }

            return true;
        }
    }
}