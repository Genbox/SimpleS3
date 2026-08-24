using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3JsonInputFormat</c> API.</summary>
public class S3JsonInputFormat(JsonType jsonType) : S3InputFormat
{
    /// <summary>Provides the <c>JsonType</c> API.</summary>
    public JsonType JsonType { get; set; } = jsonType;

    internal override void Reset()
    {
        JsonType = JsonType.Unknown;
        base.Reset();
    }
}