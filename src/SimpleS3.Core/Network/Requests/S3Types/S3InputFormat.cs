using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    public abstract class S3InputFormat
    {
        public CompressionType CompressionType { get; set; }

        internal abstract void Reset();
    }
}