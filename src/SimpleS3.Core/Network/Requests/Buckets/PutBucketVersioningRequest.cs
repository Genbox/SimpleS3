using System;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>Sets the versioning state of an existing bucket. To set the versioning state, you must be the bucket owner.</summary>
    public class PutBucketVersioningRequest : BaseRequest, IHasBucketName, IContentMd5Config, IHasMfa
    {
        internal PutBucketVersioningRequest() : base(HttpMethod.PUT)
        {
            Mfa = new MfaAuthenticationBuilder();
        }

        public PutBucketVersioningRequest(string bucketName, bool enabled) : this()
        {
            Initialize(bucketName, enabled);
        }

        public bool Status { get; set; }
        public Func<bool> ForceContentMd5 => () => true;
        public byte[]? ContentMd5 { get; set; }

        public string BucketName { get; set; }
        public MfaAuthenticationBuilder Mfa { get; }

        internal void Initialize(string bucketName, bool enabled)
        {
            BucketName = bucketName;
            Status = enabled;
        }

        public override void Reset()
        {
            ContentMd5 = null;
            Mfa.Reset();
            base.Reset();
        }
    }
}