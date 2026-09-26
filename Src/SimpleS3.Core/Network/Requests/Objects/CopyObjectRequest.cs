using Genbox.HttpBuilders;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>
/// Creates a copy of an object that is already stored in Amazon S3. When copying an object, you can preserve all metadata (default) or specify new metadata. However, the ACL
/// is not preserved and is set to private for the user making the request. To override the default ACL setting, specify a new ACL when generating a copy request.
/// </summary>
public sealed class CopyObjectRequest : BaseRequest, IHasObjectAcl, IHasCache, IHasMetadata, IHasTags, IHasLock, IHasSse, IHasBucketKeyEnabled, IHasSseCustomerKey, IHasStorageClass, IHasRequestPayer, IHasWebsiteRedirect, IHasVersionId, IHasBucketName, IHasObjectKey, IHasLegalHold, IAutoMapConfig
{
    private byte[]? _sourceSseCustomerKey;
    private byte[]? _sseCustomerKey;

    internal CopyObjectRequest() : base(HttpMethodType.PUT)
    {
        MetadataDirective = MetadataDirective.Copy;
        TaggingDirective = TaggingDirective.Copy;

        AclGrantRead = new AclBuilder();
        AclGrantReadAcp = new AclBuilder();
        AclGrantWriteAcp = new AclBuilder();
        AclGrantFullControl = new AclBuilder();
        IfETagNotMatch = new ETagBuilder();
        IfETagMatch = new ETagBuilder();
        Metadata = new MetadataBuilder();
        Tags = new TagBuilder(10);
        SseContext = new KmsContextBuilder();
    }

    /// <summary>Provides the <c>CopyObjectRequest</c> API.</summary>
    public CopyObjectRequest(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey) : this()
    {
        Initialize(sourceBucketName, sourceObjectKey, destinationBucketName, destinationObjectKey);
    }

    /// <summary>Provides the <c>SourceBucketName</c> API.</summary>
    public string SourceBucketName { get; private set; } = null!;

    /// <summary>The expected source bucket owner account ID. ExpectedBucketOwner applies to the destination bucket.</summary>
    public string? ExpectedSourceBucketOwner { get; set; }

    /// <summary>Provides the <c>SourceObjectKey</c> API.</summary>
    public string SourceObjectKey { get; private set; } = null!;

    /// <summary>Provides the <c>DestinationBucketName</c> API.</summary>
    public string DestinationBucketName { get; private set; } = null!;

    /// <summary>Provides the <c>DestinationObjectKey</c> API.</summary>
    public string DestinationObjectKey { get; private set; } = null!;

    /// <summary>Provides the <c>MetadataDirective</c> API.</summary>
    public MetadataDirective MetadataDirective { get; set; }

    /// <summary>Provides the <c>TaggingDirective</c> API.</summary>
    public TaggingDirective TaggingDirective { get; set; }

    /// <summary>Provides the <c>AnnotationDirective</c> API.</summary>
    public AnnotationDirective AnnotationDirective { get; set; }

    /// <summary>Provides the <c>SourceSseCustomerAlgorithm</c> API.</summary>
    public SseCustomerAlgorithm SourceSseCustomerAlgorithm { get; set; }

    /// <summary>Provides the <c>SourceSseCustomerKeyMd5</c> API.</summary>
    public byte[]? SourceSseCustomerKeyMd5 { get; set; }

    /// <summary>Provides the <c>SourceSseCustomerKey</c> API.</summary>
    public byte[]? SourceSseCustomerKey
    {
        get => _sourceSseCustomerKey;
        set => CopyHelper.ReplaceAndClear(ref _sourceSseCustomerKey, value);
    }

    // Copy conditions and version identify the source and must only use copy-source headers.
    /// <summary>Provides the <c>AutoMapDisabledFor</c> API.</summary>
    public Func<Type, bool> AutoMapDisabledFor => type => type == typeof(IHasVersionId) || type == typeof(IHasCache);

    /// <summary>Specifies whether Amazon S3 should use an S3 Bucket Key for server-side encryption with AWS KMS.</summary>
    public bool? BucketKeyEnabled { get; set; }

    string IHasBucketName.BucketName => DestinationBucketName;

    /// <summary>Provides the <c>IfModifiedSince</c> API.</summary>
    public DateTimeOffset? IfModifiedSince { get; set; }

    /// <summary>Provides the <c>IfUnmodifiedSince</c> API.</summary>
    public DateTimeOffset? IfUnmodifiedSince { get; set; }

    /// <summary>Provides the <c>IfETagMatch</c> API.</summary>
    public ETagBuilder IfETagMatch { get; }

    /// <summary>Provides the <c>IfETagNotMatch</c> API.</summary>
    public ETagBuilder IfETagNotMatch { get; }

    /// <summary>Provides the <c>LockLegalHold</c> API.</summary>
    public bool? LockLegalHold { get; set; }

    /// <summary>Provides the <c>LockMode</c> API.</summary>
    public LockMode LockMode { get; set; }

    /// <summary>Provides the <c>LockRetainUntil</c> API.</summary>
    public DateTimeOffset? LockRetainUntil { get; set; }

    /// <summary>Provides the <c>Metadata</c> API.</summary>
    public MetadataBuilder Metadata { get; }

    /// <summary>Provides the <c>Acl</c> API.</summary>
    public ObjectCannedAcl Acl { get; set; }

    /// <summary>Provides the <c>AclGrantRead</c> API.</summary>
    public AclBuilder AclGrantRead { get; }

    /// <summary>Provides the <c>AclGrantReadAcp</c> API.</summary>
    public AclBuilder AclGrantReadAcp { get; }

    /// <summary>Provides the <c>AclGrantWriteAcp</c> API.</summary>
    public AclBuilder AclGrantWriteAcp { get; }

    /// <summary>Provides the <c>AclGrantFullControl</c> API.</summary>
    public AclBuilder AclGrantFullControl { get; }

    string IHasObjectKey.ObjectKey => DestinationObjectKey;

    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    /// <summary>Provides the <c>SseAlgorithm</c> API.</summary>
    public SseAlgorithm SseAlgorithm { get; set; }

    /// <summary>Provides the <c>SseKmsKeyId</c> API.</summary>
    public string? SseKmsKeyId { get; set; }

    /// <summary>Provides the <c>SseContext</c> API.</summary>
    public KmsContextBuilder SseContext { get; }

    /// <summary>Provides the <c>SseCustomerAlgorithm</c> API.</summary>
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }

    /// <summary>Provides the <c>SseCustomerKey</c> API.</summary>
    public byte[]? SseCustomerKey
    {
        get => _sseCustomerKey;
        set => CopyHelper.ReplaceAndClear(ref _sseCustomerKey, value);
    }

    /// <summary>Provides the <c>ClearSensitiveMaterial</c> API.</summary>
    public void ClearSensitiveMaterial()
    {
        if (_sseCustomerKey != null)
        {
            Array.Clear(_sseCustomerKey, 0, _sseCustomerKey.Length);
            _sseCustomerKey = null;
        }

        if (_sourceSseCustomerKey != null)
        {
            Array.Clear(_sourceSseCustomerKey, 0, _sourceSseCustomerKey.Length);
            _sourceSseCustomerKey = null;
        }
    }

    /// <summary>Provides the <c>SseCustomerKeyMd5</c> API.</summary>
    public byte[]? SseCustomerKeyMd5 { get; set; }

    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    public StorageClass StorageClass { get; set; }

    /// <summary>Provides the <c>Tags</c> API.</summary>
    public TagBuilder Tags { get; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; set; }

    /// <summary>Provides the <c>WebsiteRedirectLocation</c> API.</summary>
    public string? WebsiteRedirectLocation { get; set; }

    internal void Initialize(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey)
    {
        SourceBucketName = sourceBucketName;
        SourceObjectKey = sourceObjectKey;
        DestinationBucketName = destinationBucketName;
        DestinationObjectKey = destinationObjectKey;
    }

    internal override void Reset()
    {
        ClearSensitiveMaterial();

        MetadataDirective = MetadataDirective.Copy;
        TaggingDirective = TaggingDirective.Copy;
        AnnotationDirective = AnnotationDirective.Unknown;
        IfModifiedSince = null;
        IfUnmodifiedSince = null;
        IfETagMatch.Reset();
        IfETagNotMatch.Reset();
        LockMode = LockMode.Unknown;
        LockRetainUntil = null;
        LockLegalHold = null;
        Metadata.Reset();
        Acl = ObjectCannedAcl.Unknown;
        AclGrantRead.Reset();
        AclGrantReadAcp.Reset();
        AclGrantWriteAcp.Reset();
        AclGrantFullControl.Reset();
        RequestPayer = Payer.Unknown;
        SseAlgorithm = SseAlgorithm.Unknown;
        BucketKeyEnabled = null;
        SseKmsKeyId = null;
        SseContext.Reset();
        SseCustomerAlgorithm = SseCustomerAlgorithm.Unknown;
        SseCustomerKeyMd5 = null;
        SourceSseCustomerAlgorithm = SseCustomerAlgorithm.Unknown;
        SourceSseCustomerKeyMd5 = null;
        StorageClass = StorageClass.Unknown;
        Tags.Reset();
        VersionId = null;
        WebsiteRedirectLocation = null;
        ExpectedSourceBucketOwner = null;

        SourceBucketName = null!;
        SourceObjectKey = null!;
        DestinationBucketName = null!;
        DestinationObjectKey = null!;
        base.Reset();
    }
}