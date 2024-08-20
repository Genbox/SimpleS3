using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public class S3JsonInputFormat(JsonType jsonType) : S3InputFormat
{
    public JsonType JsonType { get; set; } = jsonType;

    internal override void Reset()
    {
        JsonType = JsonType.Unknown;
    }
}