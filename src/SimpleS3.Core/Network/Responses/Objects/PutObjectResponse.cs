using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Properties;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class PutObjectResponse : BaseResponse, IHasExpiresOn, IHasETag, IHasSse, IHasSseCustomerKey, IHasStorageClass, IHasRequestCharged, IHasVersionId, IHasSseContext
    {
        public string ExpiresOn { get; internal set; }
        public string ETag { get; internal set; }
        public SseAlgorithm SseAlgorithm { get; internal set; }
        public string SseKmsKeyId { get; internal set; }
        public string SseContext { get; internal set; }
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }
        public byte[] SseCustomerKeyMd5 { get; internal set; }
        public StorageClass StorageClass { get; internal set; }
        public bool RequestCharged { get; internal set; }
        public string VersionId { get; internal set; }
    }
}