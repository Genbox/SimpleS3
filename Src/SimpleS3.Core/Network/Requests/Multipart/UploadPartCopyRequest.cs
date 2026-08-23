using Genbox.HttpBuilders;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart;

public sealed class UploadPartCopyRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasUploadId, IHasPartNumber, IHasRequestPayer, IHasSseCustomerKey
{
    private byte[]? _sseCustomerKey;
    private byte[]? _sourceSseCustomerKey;

    internal UploadPartCopyRequest() : base(HttpMethodType.PUT)
    {
        CopySourceIfETagMatch = new ETagBuilder();
        CopySourceIfETagNotMatch = new ETagBuilder();
    }

    public UploadPartCopyRequest(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey, string uploadId, int partNumber) : this()
    {
        Initialize(sourceBucketName, sourceObjectKey, destinationBucketName, destinationObjectKey, uploadId, partNumber);
    }

    public string BucketName { get; set; } = null!;
    public ETagBuilder CopySourceIfETagMatch { get; }
    public ETagBuilder CopySourceIfETagNotMatch { get; }
    public DateTimeOffset? CopySourceIfModifiedSince { get; set; }
    public DateTimeOffset? CopySourceIfUnmodifiedSince { get; set; }
    public string? CopySourceRange { get; set; }
    public string ObjectKey { get; set; } = null!;
    public int? PartNumber { get; set; }
    public Payer RequestPayer { get; set; }
    public string SourceBucketName { get; private set; } = null!;
    public string SourceObjectKey { get; private set; } = null!;
    public string UploadId { get; set; } = null!;
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }
    public byte[]? SseCustomerKeyMd5 { get; set; }
    public SseCustomerAlgorithm SourceSseCustomerAlgorithm { get; set; }
    public byte[]? SourceSseCustomerKeyMd5 { get; set; }

    public byte[]? SseCustomerKey
    {
        get => _sseCustomerKey;
        set => _sseCustomerKey = CopyHelper.CopyWithNull(value);
    }

    public byte[]? SourceSseCustomerKey
    {
        get => _sourceSseCustomerKey;
        set => _sourceSseCustomerKey = CopyHelper.CopyWithNull(value);
    }

    public void ClearSensitiveMaterial()
    {
        if (_sseCustomerKey != null)
        {
            Array.Clear(_sseCustomerKey, 0, _sseCustomerKey.Length);
            _sseCustomerKey = null;
        }

        if (_sourceSseCustomerKey != null)
        {
            Array.Clear(_sourceSseCustomerKey, 0, _sourceSseCustomerKey.Length);
            _sourceSseCustomerKey = null;
        }
    }

    internal void Initialize(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey, string uploadId, int partNumber)
    {
        if (partNumber is <= 0 or > 10_000)
            throw new ArgumentException("Part number must be between 1 and 10.000 inclusive", nameof(partNumber));

        SourceBucketName = sourceBucketName;
        SourceObjectKey = sourceObjectKey;
        BucketName = destinationBucketName;
        ObjectKey = destinationObjectKey;
        UploadId = uploadId;
        PartNumber = partNumber;
    }

    public override void Reset()
    {
        ClearSensitiveMaterial();
        CopySourceIfETagMatch.Reset();
        CopySourceIfETagNotMatch.Reset();
        CopySourceIfModifiedSince = null;
        CopySourceIfUnmodifiedSince = null;
        CopySourceRange = null;
        PartNumber = null;
        RequestPayer = Payer.Unknown;
        SseCustomerAlgorithm = SseCustomerAlgorithm.Unknown;
        SseCustomerKeyMd5 = null;
        SourceSseCustomerAlgorithm = SseCustomerAlgorithm.Unknown;
        SourceSseCustomerKeyMd5 = null;
        base.Reset();
    }
}