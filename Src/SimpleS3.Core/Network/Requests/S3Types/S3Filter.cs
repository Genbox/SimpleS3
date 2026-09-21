namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Identifies matching objects using one predicate or AndConditions.</summary>
public class S3Filter
{
    /// <summary>Provides the <c>Prefix</c> API.</summary>
    public string? Prefix { get; set; }

    /// <summary>Provides the <c>Tag</c> API.</summary>
    public KeyValuePair<string, string>? Tag { get; set; }

    /// <summary>Provides the <c>AndConditions</c> API.</summary>
    public S3AndCondition? AndConditions { get; set; }

    /// <summary>Exclusive minimum object size in bytes for a lifecycle rule.</summary>
    public long? ObjectSizeGreaterThan { get; set; }

    /// <summary>Exclusive maximum object size in bytes for a lifecycle rule.</summary>
    public long? ObjectSizeLessThan { get; set; }
}