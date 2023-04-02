using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public class S3JsonInputFormat : S3InputFormat
{
    public S3JsonInputFormat(JsonType jsonType)
    {
        JsonType = jsonType;
    }

    public JsonType JsonType { get; set; }

    internal override void Reset()
    {
        JsonType = JsonType.Unknown;
    }
}