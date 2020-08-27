using System;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>Sets an object's current Legal Hold status.</summary>
    public sealed class PutObjectLegalHoldRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IContentMd5Config, IHasLegalHold
    {
        internal PutObjectLegalHoldRequest() : base(HttpMethod.PUT)
        {
        }

        public PutObjectLegalHoldRequest(string bucketName, string objectKey, bool legalHold) : this()
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
            LockLegalHold = legalHold;
        }

        public string BucketName { get; set; }
        public string ObjectKey { get; set; }
        public Payer RequestPayer { get; set; }
        public string? VersionId { get; set; }
        public byte[]? ContentMd5 { get; set; }
        public bool? LockLegalHold { get; set; }

        Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

        public override void Reset()
        {
            BucketName = null!;
            ObjectKey = null!;
            RequestPayer = Payer.Unknown;
            VersionId = null;
            ContentMd5 = null;
            LockLegalHold = null;

            base.Reset();
        }
    }
}