using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3LoggingConfiguration</c> API.</summary>
public sealed class S3LoggingConfiguration(string targetBucket, string? targetPrefix)
{
    /// <summary>Provides the <c>TargetBucket</c> API.</summary>
    public string TargetBucket { get; set; } = targetBucket;

    /// <summary>Provides the <c>TargetPrefix</c> API.</summary>
    public string? TargetPrefix { get; set; } = targetPrefix;

    /// <summary>Provides the <c>TargetObjectKeyFormat</c> API.</summary>
    public S3LoggingObjectKeyFormat? TargetObjectKeyFormat { get; set; }

    /// <summary>Provides the <c>TargetGrants</c> API.</summary>
    public IList<S3Grant> TargetGrants { get; } = new List<S3Grant>();
}