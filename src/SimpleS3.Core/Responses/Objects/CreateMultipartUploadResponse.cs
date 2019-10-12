using System;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Responses.Objects.Properties;

namespace Genbox.SimpleS3.Core.Responses.Objects
{
    public class CreateMultipartUploadResponse : BaseResponse, ISseProperties
    {
        public DateTimeOffset AbortDate { get; internal set; }
        public string AbortRuleId { get; internal set; }
        public string Bucket { get; internal set; }
        public string Key { get; internal set; }
        public string UploadId { get; internal set; }
        public SseAlgorithm SseAlgorithm { get; internal set; }
        public string SseKmsKeyId { get; internal set; }
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }
        public byte[] SseCustomerKeyMd5 { get; internal set; }
    }
}