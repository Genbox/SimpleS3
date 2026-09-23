using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Provides the <c>RestoreObjectRequest</c> API.</summary>
public class RestoreObjectRequest : BaseRequest, IHasRequestPayer, IHasVersionId, IHasBucketName, IHasObjectKey
{
    internal RestoreObjectRequest() : base(HttpMethodType.POST) {}

    /// <summary>Provides the <c>RestoreObjectRequest</c> API.</summary>
    public RestoreObjectRequest(string bucketName, string objectKey) : this()
    {
        Initialize(bucketName, objectKey);
    }

    /// <summary>
    /// The number of days that you want the restored copy to exist. After the specified period, Amazon S3 deletes the temporary copy but the object remains archived in the
    /// <see cref="StorageClass.Glacier" /> or <see cref="StorageClass.DeepArchive" /> storage class that object was restored from.
    /// </summary>
    public int Days { get; set; }

    /// <summary>Set the tier you want the Glacier job to be handled with.</summary>
    public RetrievalTier GlacierTier { get; set; }

    /// <summary>Glacier retrieval tier at which the restore will be processed.</summary>
    public RetrievalTier RequestTier { get; set; }

    /// <summary>The optional description for the job.</summary>
    public string? Description { get; set; }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;

    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
    }

    /// <summary>Resets the request for reuse.</summary>
    public override void Reset()
    {
        Days = 0;
        GlacierTier = RetrievalTier.Unknown;
        RequestTier = RetrievalTier.Unknown;
        Description = null;
        RequestPayer = Payer.Unknown;
        VersionId = null;

        BucketName = null!;
        ObjectKey = null!;
        base.Reset();
    }
}