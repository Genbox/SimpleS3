using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>Returns the access control list (ACL) of an object. To use this operation, you must have READ_ACP access to the object.</summary>
    public class GetObjectAclRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer
    {
        internal GetObjectAclRequest() : base(HttpMethod.GET)
        {
        }

        public GetObjectAclRequest(string bucketName, string objectKey) : this()
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
        }

        public string BucketName { get; set; }
        public string ObjectKey { get; set; }
        public Payer RequestPayer { get; set; }
        public string? VersionId { get; set; }

        public override void Reset()
        {
            BucketName = null!;
            ObjectKey = null!;
            RequestPayer = Payer.Unknown;
            VersionId = null;

            base.Reset();
        }
    }
}