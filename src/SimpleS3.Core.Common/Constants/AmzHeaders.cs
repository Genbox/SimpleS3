namespace Genbox.SimpleS3.Core.Common.Constants;

public static class AmzHeaders
{
    #region Streaming

    public const string XAmzDecodedContentLength = "x-amz-decoded-content-length";

    #endregion

    public const string XAmzId2 = "x-amz-id-2";
    public const string XAmzWebsiteRedirectLocation = "x-amz-website-redirect-location";
    public const string XAmzReplicationStatus = "x-amz-replication-status";
    public const string XAmzRestore = "x-amz-restore";
    public const string XAmzDeleteMarker = "x-amz-delete-marker";
    public const string XAmzMfa = "x-amz-mfa";
    public const string XAmzAbortDate = "x-amz-abort-date";
    public const string XAmzAbortRuleId = "x-amz-abort-rule-id";
    public const string XAmzBucketObjectLockEnabled = "x-amz-bucket-object-lock-enabled";
    public const string XAmzPartsCount = "x-amz-mp-parts-count";
    public const string XAmzBypassGovernanceRetention = "x-amz-bypass-governance-retention";
    public const string XAmzRequestPayer = "x-amz-request-payer";
    public const string XAmzRequestCharged = "x-amz-request-charged";
    public const string XAmzRestoreOutputPath = "x-amz-restore-output-path";

    #region Requests

    public const string XAmzRequestId = "x-amz-request-id";
    public const string XAmzDate = "x-amz-date";
    public const string XAmzContentSha256 = "x-amz-content-sha256";

    #endregion

    #region Objects

    public const string XAmzStorageClass = "x-amz-storage-class";
    public const string XAmzVersionId = "x-amz-version-id";
    public const string XAmzExpiration = "x-amz-expiration";

    #endregion

    #region ServerSideEncryption

    public const string XAmzSse = "x-amz-server-side-encryption";
    public const string XAmzSseAwsKmsKeyId = "x-amz-server-side-encryption-aws-kms-key-id";
    public const string XAmzSseContext = "x-amz-server-side-encryption-context";

    public const string XAmzSseCustomerAlgorithm = "x-amz-server-side-encryption-customer-algorithm";
    public const string XAmzSseCustomerKey = "x-amz-server-side-encryption-customer-key";
    public const string XAmzSseCustomerKeyMd5 = "x-amz-server-side-encryption-customer-key-MD5";

    #endregion

    #region ObjectLocking

    public const string XAmzObjectLockMode = "x-amz-object-lock-mode";
    public const string XAmzObjectLockRetainUntilDate = "x-amz-object-lock-retain-until-date";
    public const string XAmzObjectLockLegalHold = "x-amz-object-lock-legal-hold";

    #endregion

    #region Tags

    public const string XAmzTagging = "x-amz-tagging";
    public const string XAmzTaggingCount = "x-amz-tagging-count";

    #endregion

    #region ACL

    public const string XAmzAcl = "x-amz-acl";
    public const string XAmzGrantRead = "x-amz-grant-read";
    public const string XAmzGrantReadAcp = "x-amz-grant-read-acp";
    public const string XAmzGrantWrite = "x-amz-grant-write";
    public const string XAmzGrantWriteAcp = "x-amz-grant-write-acp";
    public const string XAmzGrantFullControl = "x-amz-grant-full-control";

    #endregion

    #region CopyObject

    public const string XAmzCopySource = "x-amz-copy-source";
    public const string XAmzCopySourceIfMatch = "x-amz-copy-source-if-match";
    public const string XAmzCopySourceIfModifiedSince = "x-amz-copy-source-if-modified-since";
    public const string XAmzCopySourceIfNoneMatch = "x-amz-copy-source-if-none-match";
    public const string XAmzCopySourceIfUnmodifiedSince = "x-amz-copy-source-if-unmodified-since";
    public const string XAmzCopySourceSseCustomerAlgorithm = "x-amz-copy-source-server-side-encryption-customer-algorithm";
    public const string XAmzCopySourceSseCustomerKey = "x-amz-copy-source-server-side-encryption-customer-key";
    public const string XAmzCopySourceSseCustomerKeyMd5 = "x-amz-copy-source-server-side-encryption-customer-key-MD5";
    public const string XAmzTaggingDirective = "x-amz-tagging-directive";
    public const string XAmzMetadataDirective = "x-amz-metadata-directive";
    public const string XAmzCopySourceVersionId = "x-amz-copy-source-version-id";

    #endregion
}