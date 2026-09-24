using Genbox.HttpBuilders;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart;

/// <summary>Provides the <c>UploadPartCopyRequest</c> API.</summary>
public sealed class UploadPartCopyRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasUploadId, IHasPartNumber, IHasRequestPayer, IHasSseCustomerKey
{
    private byte[]? _sourceSseCustomerKey;
    private byte[]? _sseCustomerKey;

    internal UploadPartCopyRequest() : base(HttpMethodType.PUT)
    {
        CopySourceIfETagMatch = new ETagBuilder();
        CopySourceIfETagNotMatch = new ETagBuilder();
    }

    /// <summary>Provides the <c>UploadPartCopyRequest</c> API.</summary>
    public UploadPartCopyRequest(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey, string uploadId, int partNumber) : this()
    {
        Initialize(sourceBucketName, sourceObjectKey, destinationBucketName, destinationObjectKey, uploadId, partNumber);
    }

    /// <summary>Provides the <c>CopySourceIfETagMatch</c> API.</summary>
    public ETagBuilder CopySourceIfETagMatch { get; }

    /// <summary>Provides the <c>CopySourceIfETagNotMatch</c> API.</summary>
    public ETagBuilder CopySourceIfETagNotMatch { get; }

    /// <summary>Provides the <c>CopySourceIfModifiedSince</c> API.</summary>
    public DateTimeOffset? CopySourceIfModifiedSince { get; set; }

    /// <summary>Provides the <c>CopySourceIfUnmodifiedSince</c> API.</summary>
    public DateTimeOffset? CopySourceIfUnmodifiedSince { get; set; }

    /// <summary>Provides the <c>CopySourceRange</c> API.</summary>
    public string? CopySourceRange { get; set; }

    /// <summary>Provides the <c>SourceBucketName</c> API.</summary>
    public string SourceBucketName { get; private set; } = null!;

    /// <summary>The expected source bucket owner account ID. ExpectedBucketOwner applies to the destination bucket.</summary>
    public string? ExpectedSourceBucketOwner { get; set; }

    /// <summary>Provides the <c>SourceObjectKey</c> API.</summary>
    public string SourceObjectKey { get; private set; } = null!;

    /// <summary>Provides the <c>SourceSseCustomerAlgorithm</c> API.</summary>
    public SseCustomerAlgorithm SourceSseCustomerAlgorithm { get; set; }

    /// <summary>Provides the <c>SourceSseCustomerKeyMd5</c> API.</summary>
    public byte[]? SourceSseCustomerKeyMd5 { get; set; }

    /// <summary>Provides the <c>SourceSseCustomerKey</c> API.</summary>
    public byte[]? SourceSseCustomerKey
    {
        get => _sourceSseCustomerKey;
        set => _sourceSseCustomerKey = CopyHelper.CopyWithNull(value);
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;

    /// <summary>Provides the <c>PartNumber</c> API.</summary>
    public int? PartNumber { get; set; }

    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    /// <summary>Provides the <c>SseCustomerAlgorithm</c> API.</summary>
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }

    /// <summary>Provides the <c>SseCustomerKeyMd5</c> API.</summary>
    public byte[]? SseCustomerKeyMd5 { get; set; }

    /// <summary>Provides the <c>SseCustomerKey</c> API.</summary>
    public byte[]? SseCustomerKey
    {
        get => _sseCustomerKey;
        set => _sseCustomerKey = CopyHelper.CopyWithNull(value);
    }

    /// <summary>Provides the <c>ClearSensitiveMaterial</c> API.</summary>
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

    /// <summary>Provides the <c>UploadId</c> API.</summary>
    public string UploadId { get; set; } = null!;

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

    /// <summary>Provides the <c>Reset</c> API.</summary>
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
        ExpectedSourceBucketOwner = null;
        base.Reset();
    }
}