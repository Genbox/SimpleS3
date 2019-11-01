using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Properties;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class UploadPartResponse : BaseResponse, IHasStorageClass, IHasSse, IHasSseCustomerKey, IHasETag, IHasRequestCharged
    {
        /// <summary>Contains the part number set in the request. This is not returned from the server, this is set by SimpleS3.</summary>
        public int PartNumber { get; internal set; }

        public string ETag { get; internal set; }
        public bool RequestCharged { get; internal set; }
        public SseAlgorithm SseAlgorithm { get; internal set; }
        public string SseKmsKeyId { get; internal set; }
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }
        public byte[] SseCustomerKeyMd5 { get; internal set; }
        public StorageClass StorageClass { get; internal set; }
    }
}