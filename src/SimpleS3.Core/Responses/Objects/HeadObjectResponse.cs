using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Responses.Objects.Properties;

namespace Genbox.SimpleS3.Core.Responses.Objects
{
    public class HeadObjectResponse : BaseResponse, IMetadataProperties, IReplicationProperties, ISseProperties, IStorageClassProperties, IResponseContentProperties, ICacheProperties, ILockProperties
    {
        /// <summary>
        /// Amazon S3 returns this header if an Expiration action is configured for the object as part of the bucket's lifecycle configuration. The
        /// header value includes an "expiry-date" component and a URL-encoded "rule-id" component. Note that for versioning-enabled buckets, this header applies
        /// only to current versions; Amazon S3 does not provide a header to infer when a noncurrent version will be eligible for permanent deletion.
        /// </summary>
        public DateTimeOffset? Expiration { get; internal set; }

        /// <summary>Provides information about the object restoration operation and expiration time of the restored object copy.</summary>
        public string Restore { get; internal set; }

        /// <summary>Returns the count of the tags associated with the object. This header is returned only if the count is greater than zero.</summary>
        public int TagCount { get; internal set; }

        /// <summary>
        /// When a bucket is configured as a website, you can set this metadata on the object so the website endpoint will evaluate the request for the
        /// object as a 301 redirect to another object in the same bucket or an external URL.
        /// </summary>
        public string WebsiteRedirectLocation { get; internal set; }

        public int? NumberOfParts { get; internal set; }
        public bool IsDeleteMarker { get; internal set; }

        /// <summary>
        /// The version of the object. When you enable versioning, S3 generates a random number for objects added to a bucket. When you put an object in
        /// a bucket where versioning has been suspended, <see cref="VersionId" /> is always null.
        /// </summary>
        public string VersionId { get; internal set; }

        public string CacheControl { get; internal set; }
        public string ETag { get; internal set; }

        public LockMode LockMode { get; internal set; }
        public DateTimeOffset LockRetainUntilDate { get; internal set; }
        public bool LockLegalHold { get; internal set; }
        public IDictionary<string, string> Metadata { get; internal set; }
        public ReplicationStatus ReplicationStatus { get; internal set; }

        public string ContentType { get; internal set; }
        public string ContentDisposition { get; internal set; }
        public string ContentEncoding { get; internal set; }
        public string ContentLanguage { get; internal set; }
        public DateTimeOffset? Expires { get; internal set; }
        public DateTimeOffset? LastModified { get; internal set; }
        public SseAlgorithm SseAlgorithm { get; internal set; }
        public string SseKmsKeyId { get; internal set; }
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }
        public byte[] SseCustomerKeyMd5 { get; internal set; }
        public StorageClass StorageClass { get; internal set; }
    }
}