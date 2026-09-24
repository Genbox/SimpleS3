namespace Genbox.SimpleS3.Core.Common.Misc;

/// <summary>Provides the <c>AmzHeaders</c> API.</summary>
public static class AmzHeaders
{
    /// <summary>Provides the <c>XAmzId2</c> API.</summary>
    public const string XAmzId2 = "x-amz-id-2";
    /// <summary>Provides the <c>XAmzWebsiteRedirectLocation</c> API.</summary>
    public const string XAmzWebsiteRedirectLocation = "x-amz-website-redirect-location";
    /// <summary>Provides the <c>XAmzReplicationStatus</c> API.</summary>
    public const string XAmzReplicationStatus = "x-amz-replication-status";
    /// <summary>Provides the <c>XAmzRestore</c> API.</summary>
    public const string XAmzRestore = "x-amz-restore";
    /// <summary>Provides the <c>XAmzDeleteMarker</c> API.</summary>
    public const string XAmzDeleteMarker = "x-amz-delete-marker";
    /// <summary>Provides the <c>XAmzMfa</c> API.</summary>
    public const string XAmzMfa = "x-amz-mfa";
    /// <summary>Provides the <c>XAmzAbortDate</c> API.</summary>
    public const string XAmzAbortDate = "x-amz-abort-date";
    /// <summary>Provides the <c>XAmzAbortRuleId</c> API.</summary>
    public const string XAmzAbortRuleId = "x-amz-abort-rule-id";
    /// <summary>Provides the <c>XAmzBucketObjectLockEnabled</c> API.</summary>
    public const string XAmzBucketObjectLockEnabled = "x-amz-bucket-object-lock-enabled";
    /// <summary>Provides the <c>XAmzPartsCount</c> API.</summary>
    public const string XAmzPartsCount = "x-amz-mp-parts-count";
    /// <summary>Provides the <c>XAmzBypassGovernanceRetention</c> API.</summary>
    public const string XAmzBypassGovernanceRetention = "x-amz-bypass-governance-retention";
    /// <summary>Provides the <c>XAmzRequestPayer</c> API.</summary>
    public const string XAmzRequestPayer = "x-amz-request-payer";
    /// <summary>Provides the <c>XAmzRequestCharged</c> API.</summary>
    public const string XAmzRequestCharged = "x-amz-request-charged";

    #region Streaming

    /// <summary>Provides the <c>XAmzDecodedContentLength</c> API.</summary>
    public const string XAmzDecodedContentLength = "x-amz-decoded-content-length";
    /// <summary>Provides the <c>XAmzTrailer</c> API.</summary>
    public const string XAmzTrailer = "x-amz-trailer";

    #endregion

    #region Requests

    /// <summary>Provides the <c>XAmzRequestId</c> API.</summary>
    public const string XAmzRequestId = "x-amz-request-id";
    /// <summary>Provides the <c>XAmzDate</c> API.</summary>
    public const string XAmzDate = "x-amz-date";
    /// <summary>Provides the <c>XAmzContentSha256</c> API.</summary>
    public const string XAmzContentSha256 = "x-amz-content-sha256";
    /// <summary>Provides the <c>XAmzSecurityToken</c> API.</summary>
    public const string XAmzSecurityToken = "x-amz-security-token";

    #endregion

    #region Objects

    /// <summary>Provides the <c>XAmzStorageClass</c> API.</summary>
    public const string XAmzStorageClass = "x-amz-storage-class";
    /// <summary>Provides the <c>XAmzVersionId</c> API.</summary>
    public const string XAmzVersionId = "x-amz-version-id";
    /// <summary>Provides the <c>XAmzObjectVersionId</c> API.</summary>
    public const string XAmzObjectVersionId = "x-amz-object-version-id";
    /// <summary>Provides the <c>XAmzExpiration</c> API.</summary>
    public const string XAmzExpiration = "x-amz-expiration";

    /// <summary>Provides the <c>XAmzChecksumType</c> API.</summary>
    public const string XAmzChecksumType = "x-amz-checksum-type";
    /// <summary>Provides the <c>XAmzChecksumMode</c> API.</summary>
    public const string XAmzChecksumMode = "x-amz-checksum-mode";

    /// <summary>Provides the <c>XAmzChecksumAlgorithm</c> API.</summary>
    public const string XAmzChecksumAlgorithm = "x-amz-checksum-algorithm";
    /// <summary>Provides the <c>XAmzObjectAttributes</c> API.</summary>
    public const string XAmzObjectAttributes = "x-amz-object-attributes";
    /// <summary>Provides the <c>XAmzChecksum</c> API.</summary>
    public const string XAmzChecksum = "x-amz-checksum-"; //Needs to be combined with XAmzChecksumAlgorithm
    /// <summary>Provides the <c>XAmzChecksumCrc32</c> API.</summary>
    public const string XAmzChecksumCrc32 = "x-amz-checksum-crc32";
    /// <summary>Provides the <c>XAmzChecksumCrc32C</c> API.</summary>
    public const string XAmzChecksumCrc32C = "x-amz-checksum-crc32c";
    /// <summary>Provides the <c>XAmzChecksumCrc64Nvme</c> API.</summary>
    public const string XAmzChecksumCrc64Nvme = "x-amz-checksum-crc64nvme";
    /// <summary>Provides the <c>XAmzChecksumSha1</c> API.</summary>
    public const string XAmzChecksumSha1 = "x-amz-checksum-sha1";
    /// <summary>Provides the <c>XAmzChecksumSha256</c> API.</summary>
    public const string XAmzChecksumSha256 = "x-amz-checksum-sha256";
    /// <summary>Provides the <c>XAmzChecksumSha512</c> API.</summary>
    public const string XAmzChecksumSha512 = "x-amz-checksum-sha512";
    /// <summary>Provides the <c>XAmzChecksumMd5</c> API.</summary>
    public const string XAmzChecksumMd5 = "x-amz-checksum-md5";
    /// <summary>Provides the <c>XAmzChecksumXxHash64</c> API.</summary>
    public const string XAmzChecksumXxHash64 = "x-amz-checksum-xxhash64";
    /// <summary>Provides the <c>XAmzChecksumXxHash3</c> API.</summary>
    public const string XAmzChecksumXxHash3 = "x-amz-checksum-xxhash3";
    /// <summary>Provides the <c>XAmzChecksumXxHash128</c> API.</summary>
    public const string XAmzChecksumXxHash128 = "x-amz-checksum-xxhash128";
    /// <summary>Provides the <c>XAmzObjectIfMatch</c> API.</summary>
    public const string XAmzObjectIfMatch = "x-amz-object-if-match";

    #endregion

    #region ServerSideEncryption

    /// <summary>Provides the <c>XAmzSse</c> API.</summary>
    public const string XAmzSse = "x-amz-server-side-encryption";
    /// <summary>Provides the <c>XAmzSseAwsKmsKeyId</c> API.</summary>
    public const string XAmzSseAwsKmsKeyId = "x-amz-server-side-encryption-aws-kms-key-id";
    /// <summary>Provides the <c>XAmzSseContext</c> API.</summary>
    public const string XAmzSseContext = "x-amz-server-side-encryption-context";

    /// <summary>Provides the <c>XAmzSseCustomerAlgorithm</c> API.</summary>
    public const string XAmzSseCustomerAlgorithm = "x-amz-server-side-encryption-customer-algorithm";
    /// <summary>Provides the <c>XAmzSseCustomerKey</c> API.</summary>
    public const string XAmzSseCustomerKey = "x-amz-server-side-encryption-customer-key";
    /// <summary>Provides the <c>XAmzSseCustomerKeyMd5</c> API.</summary>
    public const string XAmzSseCustomerKeyMd5 = "x-amz-server-side-encryption-customer-key-md5";

    #endregion

    #region ObjectLocking

    /// <summary>Provides the <c>XAmzObjectLockMode</c> API.</summary>
    public const string XAmzObjectLockMode = "x-amz-object-lock-mode";
    /// <summary>Provides the <c>XAmzObjectLockRetainUntilDate</c> API.</summary>
    public const string XAmzObjectLockRetainUntilDate = "x-amz-object-lock-retain-until-date";
    /// <summary>Provides the <c>XAmzObjectLockLegalHold</c> API.</summary>
    public const string XAmzObjectLockLegalHold = "x-amz-object-lock-legal-hold";

    #endregion

    #region Tags

    /// <summary>Provides the <c>XAmzTagging</c> API.</summary>
    public const string XAmzTagging = "x-amz-tagging";
    /// <summary>Provides the <c>XAmzTaggingCount</c> API.</summary>
    public const string XAmzTaggingCount = "x-amz-tagging-count";

    #endregion

    #region ACL

    /// <summary>Provides the <c>XAmzAcl</c> API.</summary>
    public const string XAmzAcl = "x-amz-acl";
    /// <summary>Provides the <c>XAmzGrantRead</c> API.</summary>
    public const string XAmzGrantRead = "x-amz-grant-read";
    /// <summary>Provides the <c>XAmzGrantReadAcp</c> API.</summary>
    public const string XAmzGrantReadAcp = "x-amz-grant-read-acp";
    /// <summary>Provides the <c>XAmzGrantWrite</c> API.</summary>
    public const string XAmzGrantWrite = "x-amz-grant-write";
    /// <summary>Provides the <c>XAmzGrantWriteAcp</c> API.</summary>
    public const string XAmzGrantWriteAcp = "x-amz-grant-write-acp";
    /// <summary>Provides the <c>XAmzGrantFullControl</c> API.</summary>
    public const string XAmzGrantFullControl = "x-amz-grant-full-control";
    /// <summary>Provides the <c>XAmzObjectOwnership</c> API.</summary>
    public const string XAmzObjectOwnership = "x-amz-object-ownership";

    #endregion

    #region CopyObject

    /// <summary>Controls whether source annotations are copied.</summary>
    public const string XAmzObjectAnnotationDirective = "x-amz-object-annotation-directive";
    /// <summary>Provides the <c>XAmzCopySource</c> API.</summary>
    public const string XAmzCopySource = "x-amz-copy-source";
    /// <summary>Provides the <c>XAmzCopySourceIfMatch</c> API.</summary>
    public const string XAmzCopySourceIfMatch = "x-amz-copy-source-if-match";
    /// <summary>Provides the <c>XAmzCopySourceIfModifiedSince</c> API.</summary>
    public const string XAmzCopySourceIfModifiedSince = "x-amz-copy-source-if-modified-since";
    /// <summary>Provides the <c>XAmzCopySourceIfNoneMatch</c> API.</summary>
    public const string XAmzCopySourceIfNoneMatch = "x-amz-copy-source-if-none-match";
    /// <summary>Provides the <c>XAmzCopySourceIfUnmodifiedSince</c> API.</summary>
    public const string XAmzCopySourceIfUnmodifiedSince = "x-amz-copy-source-if-unmodified-since";
    /// <summary>Provides the <c>XAmzCopySourceRange</c> API.</summary>
    public const string XAmzCopySourceRange = "x-amz-copy-source-range";
    /// <summary>Provides the <c>XAmzCopySourceSseCustomerAlgorithm</c> API.</summary>
    public const string XAmzCopySourceSseCustomerAlgorithm = "x-amz-copy-source-server-side-encryption-customer-algorithm";
    /// <summary>Provides the <c>XAmzCopySourceSseCustomerKey</c> API.</summary>
    public const string XAmzCopySourceSseCustomerKey = "x-amz-copy-source-server-side-encryption-customer-key";
    /// <summary>Provides the <c>XAmzCopySourceSseCustomerKeyMd5</c> API.</summary>
    public const string XAmzCopySourceSseCustomerKeyMd5 = "x-amz-copy-source-server-side-encryption-customer-key-MD5";
    /// <summary>Provides the <c>XAmzTaggingDirective</c> API.</summary>
    public const string XAmzTaggingDirective = "x-amz-tagging-directive";
    /// <summary>Provides the <c>XAmzMetadataDirective</c> API.</summary>
    public const string XAmzMetadataDirective = "x-amz-metadata-directive";
    /// <summary>Provides the <c>XAmzCopySourceVersionId</c> API.</summary>
    public const string XAmzCopySourceVersionId = "x-amz-copy-source-version-id";

    #endregion

    #region Buckets

    /// <summary>Provides the <c>XAmzSdkCheckumAlgorithm</c> API.</summary>
    public const string XAmzSdkCheckumAlgorithm = "x-amz-sdk-checksum-algorithm";
    /// <summary>Provides the <c>XAmzConfirmRemoveSelfBucketAccess</c> API.</summary>
    public const string XAmzConfirmRemoveSelfBucketAccess = "x-amz-confirm-remove-self-bucket-access";
    /// <summary>Provides the <c>XAmzExpectedBucketOwner</c> API.</summary>
    public const string XAmzExpectedBucketOwner = "x-amz-expected-bucket-owner";
    /// <summary>Account ID of the expected copy source bucket owner.</summary>
    public const string XAmzSourceExpectedBucketOwner = "x-amz-source-expected-bucket-owner";

    #endregion
}