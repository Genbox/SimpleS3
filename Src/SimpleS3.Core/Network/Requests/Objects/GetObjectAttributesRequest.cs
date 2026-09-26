using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Provides the <c>GetObjectAttributesRequest</c> API.</summary>
public sealed class GetObjectAttributesRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IHasSseCustomerKey
{
    private byte[]? _sseCustomerKey;

    internal GetObjectAttributesRequest() : base(HttpMethodType.GET) {}

    /// <summary>Provides the <c>GetObjectAttributesRequest</c> API.</summary>
    public GetObjectAttributesRequest(string bucketName, string objectKey, IEnumerable<ObjectAttribute> objectAttributes) : this()
    {
        Initialize(bucketName, objectKey, objectAttributes);
    }

    /// <summary>Provides the <c>ObjectAttributes</c> API.</summary>
    public IEnumerable<ObjectAttribute> ObjectAttributes { get; private set; } = Array.Empty<ObjectAttribute>();

    /// <summary>Provides the <c>MaxParts</c> API.</summary>
    public int? MaxParts { get; set; }

    /// <summary>Provides the <c>PartNumberMarker</c> API.</summary>
    public int? PartNumberMarker { get; set; }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;

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
        set => CopyHelper.ReplaceAndClear(ref _sseCustomerKey, value);
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

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey, IEnumerable<ObjectAttribute> objectAttributes)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        ObjectAttributes = objectAttributes;
    }

    internal override void Reset()
    {
        ClearSensitiveMaterial();
        ObjectAttributes = Array.Empty<ObjectAttribute>();
        MaxParts = null;
        PartNumberMarker = null;
        RequestPayer = Payer.Unknown;
        SseCustomerAlgorithm = SseCustomerAlgorithm.Unknown;
        SseCustomerKeyMd5 = null;
        VersionId = null;
        BucketName = null!;
        ObjectKey = null!;
        base.Reset();
    }
}