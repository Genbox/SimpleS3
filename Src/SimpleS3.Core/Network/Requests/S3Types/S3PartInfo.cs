using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

[PublicAPI]
public class S3PartInfo(string eTag, int partNumber)
{
    public string ETag { get; } = eTag;
    public int PartNumber { get; } = partNumber;
}