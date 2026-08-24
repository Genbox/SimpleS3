using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

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
    /// <see cref="StorageClass.Glacier" /> or <see cref="StorageClass.DeepArchive" /> storage class that object was restored from. Do not use with restores that specify OutputLocation.
    /// </summary>
    public int Days { get; set; }

    /// <summary>Set the tier you want the Glacier job to be handled with. Do not use with restores that specify OutputLocation.</summary>
    public RetrievalTier GlacierTier { get; set; }

    /// <summary>Glacier retrieval tier at which the restore will be processed.</summary>
    public RetrievalTier RequestTier { get; set; }

    /// <summary>Provides the <c>RequestType</c> API.</summary>
    public RestoreRequestType RequestType { get; set; }

    /// <summary>The optional description for the job.</summary>
    public string? Description { get; set; }

    /// <summary>Provides the <c>SelectParameters</c> API.</summary>
    public S3SelectParameters? SelectParameters { get; set; }
    /// <summary>Provides the <c>OutputLocation</c> API.</summary>
    public S3OutputLocation? OutputLocation { get; set; }
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

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        SelectParameters?.Reset();
        OutputLocation?.Reset();
        Days = 0;
        GlacierTier = RetrievalTier.Unknown;
        RequestTier = RetrievalTier.Unknown;
        RequestType = RestoreRequestType.Unknown;
        Description = null;
        RequestPayer = Payer.Unknown;
        VersionId = null;

        base.Reset();
    }
}