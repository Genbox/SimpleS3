using System;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Multipart;

public class CreateMultipartUploadResponse : BaseResponse, IHasSse, IHasSseCustomerKey, IHasRequestCharged, IHasAbort, IHasSseContext, IHasUploadId, IHasBucketName, IHasObjectKey
{
    /// <summary>Name of the bucket to which the multipart upload was initiated.</summary>
    public string BucketName { get; internal set; }

    /// <summary>Object key for which the multipart upload was initiated.</summary>
    public string ObjectKey { get; internal set; }

    public DateTimeOffset? AbortsOn { get; internal set; }
    public string? AbortRuleId { get; internal set; }
    public bool RequestCharged { get; internal set; }
    public SseAlgorithm SseAlgorithm { get; internal set; }
    public string? SseKmsKeyId { get; internal set; }
    public string? SseContext { get; internal set; }
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }
    public byte[]? SseCustomerKeyMd5 { get; internal set; }
    public string UploadId { get; internal set; }
}