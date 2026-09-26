using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>HeadObjectResponse</c> API.</summary>
public class HeadObjectResponse : BaseResponse, IHasMetadata, IHasReplicationStatus, IHasSse, IHasBucketKeyEnabled, IHasStorageClass, IHasResponseContent, IHasCache, IHasSseCustomerKey, IHasLock, IHasDeleteMarker, IHasVersionId, IHasExpiresOn, IHasExpiration, IHasRestoration, IHasLegalHold, IHasWebsiteRedirect
{
    /// <summary>Provides information about the object restoration operation and expiration time of the restored object copy.</summary>
    public string? Restore { get; internal set; }

    /// <summary>The object's Intelligent-Tiering archive access tier, or Unknown when the service does not report it.</summary>
    public ArchiveStatus ArchiveStatus { get; internal set; }

    /// <summary>Returns the count of the tags associated with the object. This header is returned only if the count is greater than zero.</summary>
    public int TagCount { get; internal set; }

    /// <summary>Provides the <c>NumberOfParts</c> API.</summary>
    public int? NumberOfParts { get; internal set; }

    /// <summary>Provides the <c>ChecksumType</c> API.</summary>
    public ChecksumType ChecksumType { get; internal set; }

    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; internal set; }

    /// <summary>Provides the <c>Checksum</c> API.</summary>
    public byte[]? Checksum { get; internal set; }

    /// <summary>Whether Amazon S3 used an S3 Bucket Key for SSE-KMS.</summary>
    public bool? BucketKeyEnabled { get; internal set; }

    /// <summary>Provides the <c>CacheControl</c> API.</summary>
    public string? CacheControl { get; internal set; }

    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string? ETag { get; internal set; }

    /// <summary>Provides the <c>IsDeleteMarker</c> API.</summary>
    public bool IsDeleteMarker { get; internal set; }

    /// <summary>Provides the <c>LifeCycleExpiresOn</c> API.</summary>
    public DateTimeOffset? LifeCycleExpiresOn { get; internal set; }

    /// <summary>Provides the <c>LifeCycleRuleId</c> API.</summary>
    public string? LifeCycleRuleId { get; internal set; }

    /// <summary>Provides the <c>ExpiresOn</c> API.</summary>
    public DateTimeOffset? ExpiresOn { get; internal set; }

    /// <summary>Provides the <c>LockLegalHold</c> API.</summary>
    public bool LockLegalHold { get; internal set; }

    /// <summary>Provides the <c>LockMode</c> API.</summary>
    public LockMode LockMode { get; internal set; }

    /// <summary>Provides the <c>LockRetainUntil</c> API.</summary>
    public DateTimeOffset? LockRetainUntil { get; internal set; }

    /// <summary>Provides the <c>Metadata</c> API.</summary>
    public IDictionary<string, string>? Metadata { get; internal set; }

    /// <summary>Provides the <c>ReplicationStatus</c> API.</summary>
    public ReplicationStatus ReplicationStatus { get; internal set; }

    /// <summary>Provides the <c>ContentType</c> API.</summary>
    public string? ContentType { get; internal set; }

    /// <summary>Provides the <c>ContentDisposition</c> API.</summary>
    public string? ContentDisposition { get; internal set; }

    /// <summary>Provides the <c>ContentEncoding</c> API.</summary>
    public string? ContentEncoding { get; internal set; }

    /// <summary>Provides the <c>ContentLanguage</c> API.</summary>
    public string? ContentLanguage { get; internal set; }

    /// <summary>Provides the <c>ContentRange</c> API.</summary>
    public string? ContentRange { get; internal set; }

    /// <summary>Provides the <c>AcceptRanges</c> API.</summary>
    public string? AcceptRanges { get; internal set; }

    /// <summary>Provides the <c>LastModified</c> API.</summary>
    public DateTimeOffset? LastModified { get; internal set; }

    /// <summary>Provides the <c>RestoreExpiresOn</c> API.</summary>
    public DateTimeOffset? RestoreExpiresOn { get; }

    /// <summary>Provides the <c>RestoreInProgress</c> API.</summary>
    public bool RestoreInProgress { get; }

    /// <summary>Provides the <c>SseAlgorithm</c> API.</summary>
    public SseAlgorithm SseAlgorithm { get; internal set; }

    /// <summary>Provides the <c>SseKmsKeyId</c> API.</summary>
    public string? SseKmsKeyId { get; internal set; }

    /// <summary>Provides the <c>SseCustomerAlgorithm</c> API.</summary>
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; internal set; }

    /// <summary>Provides the <c>SseCustomerKeyMd5</c> API.</summary>
    public byte[]? SseCustomerKeyMd5 { get; internal set; }

    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    public StorageClass StorageClass { get; internal set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; internal set; }

    /// <summary>
    /// When a bucket is configured as a website, you can set this metadata on the object so the website endpoint will evaluate the request for the object as a 301 redirect to
    /// another object in the same bucket or an external URL.
    /// </summary>
    public string? WebsiteRedirectLocation { get; internal set; }
}