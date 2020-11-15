using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>
    /// Uses the acl subresource to set the access control list (ACL) permissions for an object that already exists in a bucket. You must have
    /// WRITE_ACP permission to set the ACL of an object. Depending on your application needs, you can choose to set the ACL on an object using either the
    /// request body or the headers. For example, if you have an existing application that updates a bucket ACL using the request body, you can continue to
    /// use that approach.
    /// </summary>
    public class PutObjectAclRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasObjectAcl, IHasVersionId, IHasContentMd5, IHasRequestPayer
    {
        internal PutObjectAclRequest() : base(HttpMethod.PUT)
        {
            AclGrantRead = new AclBuilder();
            AclGrantReadAcp = new AclBuilder();
            AclGrantWriteAcp = new AclBuilder();
            AclGrantFullControl = new AclBuilder();
        }

        public PutObjectAclRequest(string bucketName, string objectKey) : this()
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
        }

        public byte[]? ContentMd5 { get; set; }
        public ObjectCannedAcl Acl { get; set; }
        public AclBuilder AclGrantRead { get; }
        public AclBuilder AclGrantReadAcp { get; }
        public AclBuilder AclGrantWriteAcp { get; }
        public AclBuilder AclGrantFullControl { get; }
        public Payer RequestPayer { get; set; }
        public string? VersionId { get; set; }
        public string BucketName { get; set; }
        public string ObjectKey { get; set; }

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
            BucketName = null!;
            ObjectKey = null!;

            base.Reset();
        }
    }
}