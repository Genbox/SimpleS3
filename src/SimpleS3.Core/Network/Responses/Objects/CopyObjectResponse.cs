using System;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public class CopyObjectResponse : BaseResponse, IHasRequestCharged, IHasVersionId, IHasSse, IHasSseContext, IHasSseCustomerKey, IHasExpiration, IHasETag, IHasLastModified
{
    public string? NewVersionId { get; internal set; }
    public string? ETag { get; internal set; }
    public DateTimeOffset? LifeCycleExpiresOn { get; internal set; }
    public string? LifeCycleRuleId { get; internal set; }
    public DateTimeOffset? LastModified { get; internal set; }
    public bool RequestCharged { get; internal set; }
    public SseAlgorithm SseAlgorithm { get; internal set; }
    public string? SseKmsKeyId { get; internal set; }
    public string? SseContext { get; internal set; }
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }
    public byte[]? SseCustomerKeyMd5 { get; internal set; }
    public string? VersionId { get; internal set; }
}