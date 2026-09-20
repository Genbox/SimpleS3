namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3AndCondition</c> API.</summary>
public class S3AndCondition
{
    /// <summary>Provides the <c>Prefix</c> API.</summary>
    public string? Prefix { get; set; }

    /// <summary>Provides the <c>Tags</c> API.</summary>
    public IList<KeyValuePair<string, string>> Tags { get; internal set; } = new List<KeyValuePair<string, string>>();
}