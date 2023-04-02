using Genbox.SimpleS3.Cli.Core.Abstracts;
using Genbox.SimpleS3.Cli.Core.Enums;
using Genbox.SimpleS3.Cli.Core.Helpers;
using Genbox.SimpleS3.Cli.Core.Structs;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Cli.Core;

public class DefaultFileProvider : IFileProvider
{
    public async IAsyncEnumerable<OutPathData> GetFiles(ISimpleClient client, PathData src, PathData dst)
    {
        if (src.LocationType == LocationType.Local)
        {
            if (src.ResourceType == ResourceType.File && dst.ResourceType == ResourceType.File)
            {
                // C:\Windows\Explorer.exe -> S3://bucket/directory/file.exe
                yield return new OutPathData(src.Resource, dst.Resource);
            }
            else if (src.ResourceType == ResourceType.File && dst.ResourceType == ResourceType.Directory)
            {
                // C:\Windows\Explorer.exe -> S3://bucket/directory/
                // Take the filename from local and add combine it with the remote resource
                yield return new OutPathData(src.Resource, RemotePathHelper.Combine(dst.Resource, LocalPathHelper.GetFileName(src.Resource)));
            }
            else if (src.ResourceType == ResourceType.Directory && dst.ResourceType == ResourceType.Directory)
            {
                // C:\Windows\ -> S3://bucket/directory/
                // Take the filename from local and add combine it with the remote resource
                foreach (string file in Directory.GetFiles(src.Resource, "*", SearchOption.AllDirectories))
                {
                    string relativePath = LocalPathHelper.GetRelativePath(src.Resource, file);
                    yield return new OutPathData(file, RemotePathHelper.Combine(dst.Resource, relativePath));
                }
            }
            else
                throw new InvalidOperationException($"You cannot perform that operation from {src.Resource} to {dst.Resource}");
        }
        else
        {
            if (src.ResourceType == ResourceType.File && dst.ResourceType == ResourceType.File)
            {
                // S3://bucket/directory/file.exe -> C:\Windows\Explorer.exe
                yield return new OutPathData(src.Resource, dst.Resource);
            }
            else if (src.ResourceType == ResourceType.File && dst.ResourceType == ResourceType.Directory)
            {
                // S3://bucket/directory/explorer.exe -> C:\Windows\
                yield return new OutPathData(src.Resource, RemotePathHelper.Combine(dst.Resource, RemotePathHelper.GetFileName(src.Resource)));
            }
            else if (src.ResourceType == ResourceType.Directory && dst.ResourceType == ResourceType.Directory)
            {
                // S3://bucket/directory/ -> C:\Windows\
                await foreach (S3Object s3Object in client.ListAllObjectsAsync(src.Bucket, req => req.Prefix = src.Resource))
                {
                    string relativePath = RemotePathHelper.GetRelativePath(src.Resource, s3Object.ObjectKey);
                    yield return new OutPathData(s3Object.ObjectKey, RemotePathHelper.Combine(dst.Resource, relativePath));
                }
            }
            else
                throw new InvalidOperationException($"You cannot perform that operation from {src.Resource} to {dst.Resource}");
        }
    }
}