using System;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>Sets an object's current Legal Hold status.</summary>
    public sealed class PutObjectLegalHoldRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IHasContentMd5, IContentMd5Config
    {
        public PutObjectLegalHoldRequest(string bucketName, string objectKey, bool legalHold) : base(HttpMethod.PUT)
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
            LegalHold = legalHold;
        }

        public string BucketName { get; set; }
        public string ObjectKey { get; set; }
        public Payer RequestPayer { get; set; }
        public string VersionId { get; set; }
        public byte[] ContentMd5 { get; set; }
        public bool LegalHold { get; set; }
        Func<bool> IContentMd5Config.ForceContentMd5 => () => true;
    }
}