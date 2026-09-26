using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart;

/// <summary>This operation lists the parts that have been uploaded for a specific multipart upload.</summary>
public class ListPartsRequest : BaseRequest, IHasUploadId, IHasRequestPayer, IHasBucketName, IHasObjectKey
{
    internal ListPartsRequest() : base(HttpMethodType.GET) {}

    /// <summary>Provides the <c>ListPartsRequest</c> API.</summary>
    public ListPartsRequest(string bucketName, string objectKey, string uploadId) : this()
    {
        Initialize(bucketName, objectKey, uploadId);
    }

    /// <summary>Requests Amazon S3 to encode the response and specifies the encoding method to use.</summary>
    public EncodingType EncodingType { get; set; }

    /// <summary>Sets the maximum number of parts to return in the response body.</summary>
    public int? MaxParts { get; set; }

    /// <summary>Specifies the part after which listing should begin. Only parts with higher part numbers will be listed.</summary>
    public string? PartNumberMarker { get; set; }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;

    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    /// <summary>Provides the <c>UploadId</c> API.</summary>
    public string UploadId { get; set; } = null!;

    internal void Initialize(string bucketName, string objectKey, string uploadId)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        UploadId = uploadId;
    }

    internal override void Reset()
    {
        RequestPayer = Payer.Unknown;
        PartNumberMarker = null;
        MaxParts = null;
        EncodingType = EncodingType.Unknown;

        BucketName = null!;
        ObjectKey = null!;
        UploadId = null!;
        base.Reset();
    }
}