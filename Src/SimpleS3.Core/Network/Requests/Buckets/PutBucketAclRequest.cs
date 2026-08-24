using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>PutBucketAclRequest</c> API.</summary>
public sealed class PutBucketAclRequest : BaseRequest, IHasBucketName, IHasBucketAcl, IHasExpectedBucketOwner, IHasContentMd5
{
    internal PutBucketAclRequest() : base(HttpMethodType.PUT)
    {
        AclGrantRead = new AclBuilder();
        AclGrantWrite = new AclBuilder();
        AclGrantReadAcp = new AclBuilder();
        AclGrantWriteAcp = new AclBuilder();
        AclGrantFullControl = new AclBuilder();
    }

    /// <summary>Provides the <c>PutBucketAclRequest</c> API.</summary>
    public PutBucketAclRequest(string bucketName) : this() => Initialize(bucketName);
    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;
    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }
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
    /// <summary>Provides the <c>ExpectedBucketOwner</c> API.</summary>
    public string? ExpectedBucketOwner { get; set; }
    internal void Initialize(string bucketName) => BucketName = bucketName;

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        ContentMd5 = null;
        Acl = BucketCannedAcl.Unknown;
        AclGrantRead.Reset();
        AclGrantWrite.Reset();
        AclGrantReadAcp.Reset();
        AclGrantWriteAcp.Reset();
        AclGrantFullControl.Reset();
        ObjectOwnership = ObjectOwnership.Unknown;
        ExpectedBucketOwner = null;
        base.Reset();
    }
}