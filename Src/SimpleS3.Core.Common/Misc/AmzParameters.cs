namespace Genbox.SimpleS3.Core.Common.Misc;

/// <summary>Provides the <c>AmzParameters</c> API.</summary>
public static class AmzParameters
{
    private const string _responseHeader = "response-";

    /// <summary>Provides the <c>ResponseCacheControl</c> API.</summary>
    public const string ResponseCacheControl = _responseHeader + HttpHeaders.CacheControl;
    /// <summary>Provides the <c>ResponseExpires</c> API.</summary>
    public const string ResponseExpires = _responseHeader + HttpHeaders.Expires;
    /// <summary>Provides the <c>ResponseContentDisposition</c> API.</summary>
    public const string ResponseContentDisposition = _responseHeader + HttpHeaders.ContentDisposition;
    /// <summary>Provides the <c>ResponseContentEncoding</c> API.</summary>
    public const string ResponseContentEncoding = _responseHeader + HttpHeaders.ContentEncoding;
    /// <summary>Provides the <c>ResponseContentLanguage</c> API.</summary>
    public const string ResponseContentLanguage = _responseHeader + HttpHeaders.ContentLanguage;
    /// <summary>Provides the <c>ResponseContentType</c> API.</summary>
    public const string ResponseContentType = _responseHeader + HttpHeaders.ContentType;
    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public const string VersionId = "versionId";
    /// <summary>Provides the <c>EncodingType</c> API.</summary>
    public const string EncodingType = "encoding-type";
    /// <summary>Provides the <c>Delete</c> API.</summary>
    public const string Delete = "delete";
    /// <summary>Provides the <c>Versions</c> API.</summary>
    public const string Versions = "versions";
    /// <summary>Provides the <c>Versioning</c> API.</summary>
    public const string Versioning = "versioning";
    /// <summary>Provides the <c>Delimiter</c> API.</summary>
    public const string Delimiter = "delimiter";
    /// <summary>Provides the <c>MaxKeys</c> API.</summary>
    public const string MaxKeys = "max-keys";
    /// <summary>Provides the <c>Prefix</c> API.</summary>
    public const string Prefix = "prefix";
    /// <summary>Provides the <c>ListType</c> API.</summary>
    public const string ListType = "list-type";
    /// <summary>Provides the <c>ContinuationToken</c> API.</summary>
    public const string ContinuationToken = "continuation-token";
    /// <summary>Provides the <c>FetchOwner</c> API.</summary>
    public const string FetchOwner = "fetch-owner";
    /// <summary>Provides the <c>StartAfter</c> API.</summary>
    public const string StartAfter = "start-after";
    /// <summary>Provides the <c>MaxUploads</c> API.</summary>
    public const string MaxUploads = "max-uploads";
    /// <summary>Provides the <c>KeyMarker</c> API.</summary>
    public const string KeyMarker = "key-marker";
    /// <summary>Provides the <c>UploadIdMarker</c> API.</summary>
    public const string UploadIdMarker = "upload-id-marker";
    /// <summary>Provides the <c>MaxParts</c> API.</summary>
    public const string MaxParts = "max-parts";
    /// <summary>Provides the <c>PartNumberMarker</c> API.</summary>
    public const string PartNumberMarker = "part-number-marker";
    /// <summary>Provides the <c>UploadId</c> API.</summary>
    public const string UploadId = "uploadId";
    /// <summary>Provides the <c>PartNumber</c> API.</summary>
    public const string PartNumber = "partNumber";
    /// <summary>Provides the <c>Uploads</c> API.</summary>
    public const string Uploads = "uploads";
    /// <summary>Provides the <c>Restore</c> API.</summary>
    public const string Restore = "restore";
    /// <summary>Provides the <c>Acl</c> API.</summary>
    public const string Acl = "acl";
    /// <summary>Provides the <c>LegalHold</c> API.</summary>
    public const string LegalHold = "legal-hold";
    /// <summary>Provides the <c>ObjectLock</c> API.</summary>
    public const string ObjectLock = "object-lock";
    /// <summary>Provides the <c>Tagging</c> API.</summary>
    public const string Tagging = "tagging";
    /// <summary>Provides the <c>Attributes</c> API.</summary>
    public const string Attributes = "attributes";
    /// <summary>Provides the <c>Cors</c> API.</summary>
    public const string Cors = "cors";
    /// <summary>Provides the <c>Accelerate</c> API.</summary>
    public const string Accelerate = "accelerate";
    /// <summary>Provides the <c>Lifecycle</c> API.</summary>
    public const string Lifecycle = "lifecycle";
    /// <summary>Provides the <c>Encryption</c> API.</summary>
    public const string Encryption = "encryption";
    /// <summary>Provides the <c>Location</c> API.</summary>
    public const string Location = "location";
    /// <summary>Provides the <c>OwnershipControls</c> API.</summary>
    public const string OwnershipControls = "ownershipControls";
    /// <summary>Provides the <c>PublicAccessBlock</c> API.</summary>
    public const string PublicAccessBlock = "publicAccessBlock";
    /// <summary>Provides the <c>Retention</c> API.</summary>
    public const string Retention = "retention";
    /// <summary>Provides the <c>RequestPayment</c> API.</summary>
    public const string RequestPayment = "requestPayment";
    /// <summary>Provides the <c>VersionIdMarker</c> API.</summary>
    public const string VersionIdMarker = "version-id-marker";
    /// <summary>Provides the <c>Policy</c> API.</summary>
    public const string Policy = "policy";
    /// <summary>Provides the <c>PolicyStatus</c> API.</summary>
    public const string PolicyStatus = "policyStatus";
    /// <summary>Provides the <c>Website</c> API.</summary>
    public const string Website = "website";
    /// <summary>Provides the <c>Id</c> API.</summary>
    public const string Id = "id";
    /// <summary>Provides the <c>Analytics</c> API.</summary>
    public const string Analytics = "analytics";
    /// <summary>Provides the <c>Inventory</c> API.</summary>
    public const string Inventory = "inventory";
    /// <summary>Provides the <c>Metrics</c> API.</summary>
    public const string Metrics = "metrics";
    /// <summary>Provides the <c>IntelligentTiering</c> API.</summary>
    public const string IntelligentTiering = "intelligent-tiering";
    /// <summary>Provides the <c>Replication</c> API.</summary>
    public const string Replication = "replication";
    /// <summary>Provides the <c>Annotation</c> API.</summary>
    public const string Annotation = "annotation";
    /// <summary>Provides the <c>AnnotationName</c> API.</summary>
    public const string AnnotationName = "annotationName";
    /// <summary>Provides the <c>AnnotationPrefix</c> API.</summary>
    public const string AnnotationPrefix = "annotation-prefix";
    /// <summary>Provides the <c>MaxAnnotationResults</c> API.</summary>
    public const string MaxAnnotationResults = "max-annotation-results";
    /// <summary>Provides the <c>MetadataAnnotationTable</c> API.</summary>
    public const string MetadataAnnotationTable = "metadataAnnotationTable";

    #region Presigning

    //Note: These are actually case-sensitive
    /// <summary>Provides the <c>XAmzAlgorithm</c> API.</summary>
    public const string XAmzAlgorithm = "X-Amz-Algorithm";
    /// <summary>Provides the <c>XAmzCredential</c> API.</summary>
    public const string XAmzCredential = "X-Amz-Credential";
    /// <summary>Provides the <c>XAmzSignedHeaders</c> API.</summary>
    public const string XAmzSignedHeaders = "X-Amz-SignedHeaders";
    /// <summary>Provides the <c>XAmzSignature</c> API.</summary>
    public const string XAmzSignature = "X-Amz-Signature";
    /// <summary>Provides the <c>XAmzExpires</c> API.</summary>
    public const string XAmzExpires = "X-Amz-Expires";
    /// <summary>Provides the <c>XAmzDate</c> API.</summary>
    public const string XAmzDate = "X-Amz-Date";

    #endregion
}