using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>
/// Uses the acl subresource to set the access control list (ACL) permissions for an object that already exists in a bucket. You must have WRITE_ACP permission to set the ACL
/// of an object. Depending on your application needs, you can choose to set the ACL on an object using either the request body or the headers. For example, if you have an existing
/// application that updates a bucket ACL using the request body, you can continue to use that approach.
/// </summary>
public class PutObjectAclRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasObjectAcl, IHasVersionId, IHasContentMd5, IHasRequestPayer
{
    internal PutObjectAclRequest() : base(HttpMethodType.PUT)
    {
        AclGrantRead = new AclBuilder();
        AclGrantReadAcp = new AclBuilder();
        AclGrantWriteAcp = new AclBuilder();
        AclGrantFullControl = new AclBuilder();
    }

    /// <summary>Provides the <c>PutObjectAclRequest</c> API.</summary>
    public PutObjectAclRequest(string bucketName, string objectKey) : this()
    {
        Initialize(bucketName, objectKey);
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;
    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }
    /// <summary>Provides the <c>Acl</c> API.</summary>
    public ObjectCannedAcl Acl { get; set; }
    /// <summary>Provides the <c>AclGrantRead</c> API.</summary>
    public AclBuilder AclGrantRead { get; }
    /// <summary>Provides the <c>AclGrantReadAcp</c> API.</summary>
    public AclBuilder AclGrantReadAcp { get; }
    /// <summary>Provides the <c>AclGrantWriteAcp</c> API.</summary>
    public AclBuilder AclGrantWriteAcp { get; }
    /// <summary>Provides the <c>AclGrantFullControl</c> API.</summary>
    public AclBuilder AclGrantFullControl { get; }
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
        ContentMd5 = null;
        Acl = ObjectCannedAcl.Unknown;
        AclGrantRead.Reset();
        AclGrantReadAcp.Reset();
        AclGrantWriteAcp.Reset();
        AclGrantFullControl.Reset();
        RequestPayer = Payer.Unknown;
        VersionId = null;

        base.Reset();
    }
}