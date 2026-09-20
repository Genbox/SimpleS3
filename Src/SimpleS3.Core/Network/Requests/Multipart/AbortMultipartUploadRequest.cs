using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart;

/// <summary>
/// This operation aborts a multipart upload. After a multipart upload is aborted, no additional parts can be uploaded using that upload ID. The storage consumed by any
/// previously uploaded parts will be freed. However, if any part uploads are currently in progress, those part uploads might or might not succeed. As a result, it might be necessary
/// to abort a given multipart upload multiple times in order to completely free all storage consumed by all parts. To verify that all parts have been removed, so you don't get
/// charged for the part storage, you should call the List Parts operation and ensure the parts list is empty.
/// </summary>
public class AbortMultipartUploadRequest : BaseRequest, IHasRequestPayer, IHasBucketName, IHasObjectKey, IHasUploadId
{
    internal AbortMultipartUploadRequest() : base(HttpMethodType.DELETE) {}

    /// <summary>Provides the <c>AbortMultipartUploadRequest</c> API.</summary>
    public AbortMultipartUploadRequest(string bucketName, string objectKey, string uploadId) : this()
    {
        Initialize(bucketName, objectKey, uploadId);
    }

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

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        RequestPayer = Payer.Unknown;

        base.Reset();
    }
}