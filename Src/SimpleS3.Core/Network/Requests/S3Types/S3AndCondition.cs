namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public class S3AndCondition
{
    public string? Prefix { get; set; }
    public IList<KeyValuePair<string, string>> Tags { get; internal set; } = new List<KeyValuePair<string, string>>();
}