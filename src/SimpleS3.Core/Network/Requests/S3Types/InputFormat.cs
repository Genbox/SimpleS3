using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    public abstract class InputFormat
    {
        public CompressionType CompressionType { get; set; }
    }
}