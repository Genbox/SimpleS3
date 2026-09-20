using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart;

/// <summary>This operation uploads a part in a multipart upload.</summary>
public class UploadPartRequest : BaseRequest, IHasSseCustomerKey, IHasContentMd5, ISupportStreaming, IHasUploadId, IHasRequestPayer, IHasBucketName, IHasObjectKey, IHasPartNumber, IHasContent, IHasChecksumProperties
{
    private byte[]? _sseCustomerKey;

    internal UploadPartRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>UploadPartRequest</c> API.</summary>
    public UploadPartRequest(string bucketName, string objectKey, string uploadId, int partNumber, Stream? content) : this()
    {
        Initialize(bucketName, objectKey, uploadId, partNumber, content);
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; set; }

    /// <summary>Provides the <c>Checksum</c> API.</summary>
    public byte[]? Checksum { get; set; }

    /// <summary>Provides the <c>Content</c> API.</summary>
    public Stream? Content { get; private set; }

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

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
    }

    /// <summary>Provides the <c>UploadId</c> API.</summary>
    public string UploadId { get; set; } = null!;

    internal void Initialize(string bucketName, string objectKey, string uploadId, int partNumber, Stream? content)
    {
        if (partNumber is <= 0 or > 10_000)
            throw new ArgumentException("Part number must be between 1 and 10.000 inclusive", nameof(partNumber));

        BucketName = bucketName;
        ObjectKey = objectKey;
        PartNumber = partNumber;
        UploadId = uploadId;
        Content = content;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        ClearSensitiveMaterial();

        ContentMd5 = null;
        RequestPayer = Payer.Unknown;
        SseCustomerAlgorithm = SseCustomerAlgorithm.Unknown;
        SseCustomerKeyMd5 = null;
        ChecksumAlgorithm = ChecksumAlgorithm.Unknown;
        Checksum = null;

        base.Reset();
    }
}