using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

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

    public PutBucketAclRequest(string bucketName) : this() => Initialize(bucketName);
    public string BucketName { get; set; } = null!;
    public byte[]? ContentMd5 { get; set; }
    public BucketCannedAcl Acl { get; set; }
    public AclBuilder AclGrantRead { get; }
    public AclBuilder AclGrantWrite { get; }
    public AclBuilder AclGrantReadAcp { get; }
    public AclBuilder AclGrantWriteAcp { get; }
    public AclBuilder AclGrantFullControl { get; }
    public ObjectOwnership ObjectOwnership { get; set; }
    public string? ExpectedBucketOwner { get; set; }
    internal void Initialize(string bucketName) => BucketName = bucketName;

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