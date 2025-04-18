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

    public UploadPartRequest(string bucketName, string objectKey, string uploadId, int partNumber, Stream? content) : this()
    {
        Initialize(bucketName, objectKey, uploadId, partNumber, content);
    }

    public string BucketName { get; set; } = null!;
    public Stream? Content { get; private set; }
    public byte[]? ContentMd5 { get; set; }
    public string ObjectKey { get; set; } = null!;

    public int? PartNumber { get; set; }
    public Payer RequestPayer { get; set; }
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }
    public byte[]? SseCustomerKeyMd5 { get; set; }

    public byte[]? SseCustomerKey
    {
        get => _sseCustomerKey;
        set => _sseCustomerKey = CopyHelper.CopyWithNull(value);
    }

    public void ClearSensitiveMaterial()
    {
        if (_sseCustomerKey != null)
        {
            Array.Clear(_sseCustomerKey, 0, _sseCustomerKey.Length);
            _sseCustomerKey = null;
        }
    }

    public string UploadId { get; set; } = null!;

    public ChecksumAlgorithm ChecksumAlgorithm { get; set; }
    public byte[]? Checksum { get; set; }

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