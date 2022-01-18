namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>The Filter is used to identify objects that a Lifecycle Rule applies to. A Filter must have exactly one of Prefix, Tag, or conditions specified.</summary>
public class S3Filter
{
    public string? Prefix { get; set; }

    public KeyValuePair<string, string>? Tag { get; set; }

    public S3AndCondition? AndConditions { get; set; }
}