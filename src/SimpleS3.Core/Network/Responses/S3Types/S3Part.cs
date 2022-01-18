using System;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3Part : IHasETag
{
    public S3Part(int partNumber, DateTimeOffset lastModified, long size, string? eTag)
    {
        PartNumber = partNumber;
        LastModified = lastModified;
        Size = size;
        ETag = eTag;
    }

    public int PartNumber { get; }
    public DateTimeOffset LastModified { get; }
    public long Size { get; }
    public string? ETag { get; }

    public override string ToString()
    {
        return $"Part: {PartNumber}";
    }
}