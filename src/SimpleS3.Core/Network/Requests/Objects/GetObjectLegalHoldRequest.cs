using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>Gets an object's current Legal Hold status.</summary>
    public class GetObjectLegalHoldRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer
    {
        internal GetObjectLegalHoldRequest() : base(HttpMethod.GET) { }

        public GetObjectLegalHoldRequest(string bucketName, string objectKey) : this()
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