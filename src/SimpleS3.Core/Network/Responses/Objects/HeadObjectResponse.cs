using System;
using System.Collections.Generic;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Properties;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class HeadObjectResponse : BaseResponse, IHasMetadata, IHasReplicationStatus, IHasSse, IHasStorageClass, IHasResponseContent, IHasCache, IHasSseCustomerKey, IHasLock, IHasDeleteMarker, IHasVersionId, IHasExpiresOn, IHasExpiration
    {
        /// <summary>Provides information about the object restoration operation and expiration time of the restored object copy.</summary>
        public string Restore { get; internal set; }

        /// <summary>Returns the count of the tags associated with the object. This header is returned only if the count is greater than zero.</summary>
        public int TagCount { get; internal set; }

        /// <summary>
        /// When a bucket is configured as a website, you can set this metadata on the object so the website endpoint will evaluate the request for the
        /// object as a 301 redirect to another object in the same bucket or an external URL.
        /// </summary>
        public string WebsiteRedirectLocation { get; internal set; }

        public DateTimeOffset? LifeCycleExpiresOn { get; internal set; }
        public string LifeCycleRuleId { get; internal set; }
        public int? NumberOfParts { get; internal set; }
        public string CacheControl { get; internal set; }
        public string ETag { get; internal set; }
        public bool IsDeleteMarker { get; internal set; }
        public LockMode LockMode { get; internal set; }
        public DateTimeOffset LockRetainUntilDate { get; internal set; }
        public bool LockLegalHold { get; internal set; }
        public IDictionary<string, string> Metadata { get; internal set; }
        public ReplicationStatus ReplicationStatus { get; internal set; }
        public string ContentType { get; internal set; }
        public string ContentDisposition { get; internal set; }
        public string ContentEncoding { get; internal set; }
        public string ContentLanguage { get; internal set; }
        public string ContentRange { get; internal set; }
        public string AcceptRanges { get; internal set; }
        public DateTimeOffset? ExpiresOn { get; internal set; }
        public DateTimeOffset? LastModified { get; internal set; }
        public SseAlgorithm SseAlgorithm { get; internal set; }
        public string SseKmsKeyId { get; internal set; }
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }
        public byte[] SseCustomerKeyMd5 { get; internal set; }
        public StorageClass StorageClass { get; internal set; }
        public string VersionId { get; internal set; }
    }
}