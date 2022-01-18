using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public class S3AndCondition
{
    public S3AndCondition()
    {
        Tags = new List<KeyValuePair<string, string>>();
    }

    public string? Prefix { get; set; }

    public IList<KeyValuePair<string, string>> Tags { get; internal set; }
}