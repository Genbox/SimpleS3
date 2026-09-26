using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Annotation</c> API.</summary>
public sealed class S3Annotation
{
    internal S3Annotation(string name, DateTimeOffset lastModified, long size)
    {
        Name = name;
        LastModified = lastModified;
        Size = size;
    }

    /// <summary>Provides the <c>Name</c> API.</summary>
    public string Name { get; }

    /// <summary>Provides the <c>LastModified</c> API.</summary>
    public DateTimeOffset LastModified { get; }

    /// <summary>Provides the <c>Size</c> API.</summary>
    public long Size { get; }

    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string? ETag { get; internal set; }

    /// <summary>Provides the <c>ChecksumAlgorithms</c> API.</summary>
    public IList<ChecksumAlgorithm> ChecksumAlgorithms { get; } = new List<ChecksumAlgorithm>();

    /// <summary>Provides the <c>ReplicationStatus</c> API.</summary>
    public ReplicationStatus ReplicationStatus { get; internal set; }
}