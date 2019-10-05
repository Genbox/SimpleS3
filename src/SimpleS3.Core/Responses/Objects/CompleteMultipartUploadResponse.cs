using System;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Responses.Objects.Properties;

namespace Genbox.SimpleS3.Core.Responses.Objects
{
    public class CompleteMultipartUploadResponse : BaseResponse, ISseProperties
    {
        public DateTimeOffset Expiration { get; internal set; }

        public string Location { get; internal set; }
        public string Bucket { get; internal set; }
        public string Key { get; internal set; }
        public string ETag { get; internal set; }

        public SseAlgorithm SseAlgorithm { get; internal set; }
        public string SseKmsKeyId { get; internal set; }
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }
        public byte[] SseCustomerKeyMd5 { get; internal set; }
    }
}