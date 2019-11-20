using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    public class S3JsonInputFormat : InputFormat
    {
        public JsonType JsonType { get; set; }
    }
}