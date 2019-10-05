using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Responses.Objects.Properties;

namespace Genbox.SimpleS3.Core.Responses.Objects
{
    public class UploadPartResponse : BaseResponse, IStorageClassProperties, ISseProperties
    {
        public string ETag { get; internal set; }
        public int PartNumber { get; internal set; }
        public SseAlgorithm SseAlgorithm { get; internal set; }
        public string SseKmsKeyId { get; internal set; }
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }
        public byte[] SseCustomerKeyMd5 { get; internal set; }
        public StorageClass StorageClass { get; internal set; }
    }
}