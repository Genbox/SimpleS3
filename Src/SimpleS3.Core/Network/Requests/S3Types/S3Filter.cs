namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>The Filter is used to identify objects that a Lifecycle Rule applies to. A Filter must have exactly one of Prefix, Tag, or conditions specified.</summary>
public class S3Filter
{
    /// <summary>Provides the <c>Prefix</c> API.</summary>
    public string? Prefix { get; set; }

    /// <summary>Provides the <c>Tag</c> API.</summary>
    public KeyValuePair<string, string>? Tag { get; set; }

    /// <summary>Provides the <c>AndConditions</c> API.</summary>
    public S3AndCondition? AndConditions { get; set; }
}