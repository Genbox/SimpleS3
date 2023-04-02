using Genbox.SimpleS3.Cli.Core.Enums;
using Genbox.SimpleS3.Cli.Core.Structs;

namespace Genbox.SimpleS3.Cli.Core.Helpers;

public static class ResourceHelper
{
    /// <summary>
    /// Parses an input path into a remote or local file/directory. If the input starts with s3:// the bucket will be parsed and the resource will be relative. If the input is a local path, bucket will be null and the resource will be an absolute path.>
    /// </summary>
    /// <param name="path">The input path</param>
    /// <param name="pathData">The parsed data from path</param>
    /// <param name="basePath">An optional base path where relative paths will be resolved from</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static bool TryParsePath(string path, out PathData pathData, string? basePath = null)
    {
        LocationType locationType = path.StartsWith("s3://", StringComparison.OrdinalIgnoreCase) ? LocationType.Remote : LocationType.Local;
        ResourceType resourceType;

        if (locationType == LocationType.Local)
        {
            if (basePath == null)
                basePath = Environment.CurrentDirectory;

            string basePathRoot = GetRoot(basePath);

            if (basePathRoot == string.Empty)
                throw new InvalidOperationException($"The base-path '{basePath}' must be rooted");

            if (!Directory.Exists(basePath))
                throw new InvalidOperationException($"The base-path '{basePath}' does not exist");

            string root = GetRoot(path);
            string fullPath = root == string.Empty ? LocalPathHelper.Combine(basePath, path) : path;

            if (Directory.Exists(fullPath) || File.Exists(fullPath))
            {
                FileAttributes attr = File.GetAttributes(fullPath);
                resourceType = attr.HasFlag(FileAttributes.Directory) ? ResourceType.Directory : ResourceType.File;
            }
            else
                resourceType = fullPath.EndsWith('\\') ? ResourceType.Directory : ResourceType.File;

            pathData = new PathData(string.Empty, fullPath, locationType, resourceType);
        }
        else
        {
            int indexOfSlash = path.IndexOf('/', 5);

            string parsedResource;
            if (indexOfSlash != -1)
            {
                parsedResource = path.Substring(indexOfSlash + 1);

                if (parsedResource.EndsWith('*'))
                {
                    resourceType = ResourceType.Expand;
                    parsedResource = parsedResource.TrimEnd('*');
                }
                else if (parsedResource.EndsWith('/') || parsedResource.Length == 0)
                    resourceType = ResourceType.Directory;
                else
                    resourceType = ResourceType.File;
            }
            else
            {
                indexOfSlash = path.Length;
                parsedResource = string.Empty;
                resourceType = ResourceType.Directory;
            }

            string parsedBucket = path.Substring(5, indexOfSlash - 5);

            pathData = new PathData(parsedBucket, parsedResource, locationType, resourceType);
        }

        return true;
    }

    private static string GetRoot(string path)
    {
        string? root = Path.GetPathRoot(path);

        if (root == null)
            return string.Empty;

        return root;
    }
}