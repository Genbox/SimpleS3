using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

public sealed class GetObjectAttributesRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IHasSseCustomerKey, IHasExpectedBucketOwner
{
    private byte[]? _sseCustomerKey;

    internal GetObjectAttributesRequest() : base(HttpMethodType.GET) {}
    public GetObjectAttributesRequest(string bucketName, string objectKey, IEnumerable<ObjectAttribute> objectAttributes) : this() => Initialize(bucketName, objectKey, objectAttributes);
    public string BucketName { get; set; } = null!;
    public string ObjectKey { get; set; } = null!;
    public IEnumerable<ObjectAttribute> ObjectAttributes { get; private set; } = Array.Empty<ObjectAttribute>();
    public int? MaxParts { get; set; }
    public int? PartNumberMarker { get; set; }
    public Payer RequestPayer { get; set; }
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }
    public byte[]? SseCustomerKeyMd5 { get; set; }
    public string? VersionId { get; set; }
    public string? ExpectedBucketOwner { get; set; }

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

    internal void Initialize(string bucketName, string objectKey, IEnumerable<ObjectAttribute> objectAttributes)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        ObjectAttributes = objectAttributes;
    }

    public override void Reset()
    {
        ClearSensitiveMaterial();
        ObjectAttributes = Array.Empty<ObjectAttribute>();
        MaxParts = null;
        PartNumberMarker = null;
        RequestPayer = Payer.Unknown;
        SseCustomerAlgorithm = SseCustomerAlgorithm.Unknown;
        SseCustomerKeyMd5 = null;
        VersionId = null;
        ExpectedBucketOwner = null;
        base.Reset();
    }
}