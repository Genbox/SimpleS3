using System;
using System.Collections.Generic;
using System.Linq;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>
    /// The Multi-Object Delete operation enables you to delete multiple objects from a bucket using a single HTTP request. If you know the object
    /// keys that you want to delete, then this operation provides a suitable alternative to sending individual delete requests (see DELETE Object), reducing
    /// per-request overhead.
    /// </summary>
    public sealed class DeleteObjectsRequest : BaseRequest, IHasRequestPayer, IHasBypassGovernanceRetention, IHasBucketName, IHasMfa, IContentMd5Config
    {
        internal DeleteObjectsRequest() : base(HttpMethod.POST)
        {
            Mfa = new MfaAuthenticationBuilder();
            Quiet = true;
        }

        public DeleteObjectsRequest(string bucketName, IEnumerable<S3DeleteInfo> resources) : this()
        {
            BucketName = bucketName;
            Objects = resources.ToList();
        }

        /// <summary>In quiet mode the response includes only keys where the delete operation encountered an error.</summary>
        public bool Quiet { get; set; }

        /// <summary>The list of objects</summary>
        public IList<S3DeleteInfo> Objects { get; internal set; }

        public string BucketName { get; set; }
        public bool? BypassGovernanceRetention { get; set; }
        public byte[]? ContentMd5 { get; set; }
        public MfaAuthenticationBuilder Mfa { get; }
        public Payer RequestPayer { get; set; }
        Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

        public override void Reset()
        {
            BucketName = null!;
            Objects = null!;
            Mfa.Reset();
            Quiet = true;
            BypassGovernanceRetention = null;
            ContentMd5 = null;
            RequestPayer = Payer.Unknown;

            base.Reset();
        }
    }
}