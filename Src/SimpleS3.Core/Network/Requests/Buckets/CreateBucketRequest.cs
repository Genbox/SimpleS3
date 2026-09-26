using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>
/// Create a bucket. By creating the bucket, you become the bucket owner. By default, the bucket is created in the US East (N. Virginia) region. You can optionally specify a
/// region in the request body. You might choose a region to optimize latency, minimize costs, or address regulatory requirements.
/// </summary>
public class CreateBucketRequest : BaseRequest, IHasBucketAcl, IHasBucketName
{
    internal CreateBucketRequest() : base(HttpMethodType.PUT)
    {
        AclGrantRead = new AclBuilder();
        AclGrantWrite = new AclBuilder();
        AclGrantReadAcp = new AclBuilder();
        AclGrantWriteAcp = new AclBuilder();
        AclGrantFullControl = new AclBuilder();
    }

    /// <summary>Provides the <c>CreateBucketRequest</c> API.</summary>
    public CreateBucketRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>The namespace for the bucket name. Unknown omits the header.</summary>
    public BucketNamespace BucketNamespace { get; set; }

    /// <summary>Enable object locking on the bucket.</summary>
    public bool? EnableObjectLocking { get; set; }

    /// <summary>Provides the <c>Acl</c> API.</summary>
    public BucketCannedAcl Acl { get; set; }

    /// <summary>Provides the <c>AclGrantRead</c> API.</summary>
    public AclBuilder AclGrantRead { get; }

    /// <summary>Provides the <c>AclGrantWrite</c> API.</summary>
    public AclBuilder AclGrantWrite { get; }

    /// <summary>Provides the <c>AclGrantReadAcp</c> API.</summary>
    public AclBuilder AclGrantReadAcp { get; }

    /// <summary>Provides the <c>AclGrantWriteAcp</c> API.</summary>
    public AclBuilder AclGrantWriteAcp { get; }

    /// <summary>Provides the <c>AclGrantFullControl</c> API.</summary>
    public AclBuilder AclGrantFullControl { get; }

    /// <summary>Provides the <c>ObjectOwnership</c> API.</summary>
    public ObjectOwnership ObjectOwnership { get; set; }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName) => BucketName = bucketName;

    internal override void Reset()
    {
        EnableObjectLocking = null;
        Acl = BucketCannedAcl.Unknown;
        AclGrantRead.Reset();
        AclGrantWrite.Reset();
        AclGrantReadAcp.Reset();
        AclGrantWriteAcp.Reset();
        AclGrantFullControl.Reset();
        ObjectOwnership = ObjectOwnership.Unknown;

        BucketNamespace = BucketNamespace.Unknown;
        BucketName = null!;
        base.Reset();
    }
}