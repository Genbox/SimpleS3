using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>
    /// Sets the tags for a bucket. Use tags to organize your AWS bill to reflect your own cost structure. To do this, sign up to get your AWS
    /// account bill with tag key values included. Then, to see the cost of combined resources, organize your billing information according to resources with
    /// the same tag key values. For example, you can tag several resources with a specific application name, and then organize your billing information to
    /// see the total cost of that application across several services. To use this operation, you must have permissions to perform the s3:PutBucketTagging
    /// action. The bucket owner has this permission by default and can grant this permission to others.
    /// </summary>
    public class PutBucketTaggingRequest : BaseRequest, IHasBucketName, IContentMd5Config, IHasTags
    {
        internal PutBucketTaggingRequest() : base(HttpMethod.PUT)
        {
            Tags = new TagBuilder();
        }

        public PutBucketTaggingRequest(string bucketName, IDictionary<string, string> tags) : this()
        {
            Initialize(bucketName, tags);
        }

        internal void Initialize(string bucketName, IDictionary<string, string> tags)
        {
            BucketName = bucketName;

            foreach (KeyValuePair<string, string> pair in tags)
            {
                Tags.Add(pair.Key, pair.Value);
            }
        }

        public string BucketName { get; set; }
        public byte[]? ContentMd5 { get; set; }
        public Func<bool> ForceContentMd5 => () => true;
        public TagBuilder Tags { get; }

        public override void Reset()
        {
            ContentMd5 = null;
            Tags.Reset();

            base.Reset();
        }
    }
}