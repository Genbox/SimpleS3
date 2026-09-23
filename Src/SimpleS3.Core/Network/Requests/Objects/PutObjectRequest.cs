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
/// This implementation of the PUT operation adds an object to a bucket. You must have WRITE permissions on a bucket to add an object to it. Amazon S3 never adds partial
/// objects; if you receive a success response, Amazon S3 added the entire object to the bucket.
/// </summary>
public sealed class PutObjectRequest : BaseRequest, IHasContentProps, IHasExpiresOn, IHasCacheControl, IHasStorageClass, IHasLock, IHasObjectAcl, IHasSse, IHasSseCustomerKey, IHasRequestPayer, IHasBucketName, IHasObjectKey, IHasWebsiteRedirect, IHasMetadata, IHasTags, IHasLegalHold, IHasContent, ISupportStreaming, IContentMd5Config, IHasChecksumProperties
{
    private byte[]? _sseCustomerKey;

    internal PutObjectRequest() : base(HttpMethodType.PUT)
    {
        Tags = new TagBuilder(10);
        Metadata = new MetadataBuilder();
        CacheControl = new CacheControlBuilder();
        ContentDisposition = new ContentDispositionBuilder();
        ContentEncoding = new ContentEncodingBuilder();
        ContentType = new ContentTypeBuilder();
        AclGrantRead = new AclBuilder();
        AclGrantReadAcp = new AclBuilder();
        AclGrantWriteAcp = new AclBuilder();
        AclGrantFullControl = new AclBuilder();
        SseContext = new KmsContextBuilder();
    }

    /// <summary>Provides the <c>PutObjectRequest</c> API.</summary>
    public PutObjectRequest(string bucketName, string objectKey, Stream? content) : this()
    {
        Initialize(bucketName, objectKey, content);
    }

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => LockLegalHold == true || LockMode != LockMode.Unknown;

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>CacheControl</c> API.</summary>
    public CacheControlBuilder CacheControl { get; internal set; }

    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; set; }

    /// <summary>Provides the <c>Checksum</c> API.</summary>
    public byte[]? Checksum { get; set; }

    /// <summary>Provides the <c>Content</c> API.</summary>
    public Stream? Content { get; internal set; }

    /// <summary>Provides the <c>ContentDisposition</c> API.</summary>
    public ContentDispositionBuilder ContentDisposition { get; internal set; }

    /// <summary>Provides the <c>ContentEncoding</c> API.</summary>
    public ContentEncodingBuilder ContentEncoding { get; internal set; }

    /// <summary>Provides the <c>ContentType</c> API.</summary>
    public ContentTypeBuilder ContentType { get; internal set; }

    /// <summary>Provides the <c>ExpiresOn</c> API.</summary>
    public DateTimeOffset? ExpiresOn { get; set; }

    /// <summary>Provides the <c>LockLegalHold</c> API.</summary>
    public bool? LockLegalHold { get; set; }

    /// <summary>Provides the <c>LockMode</c> API.</summary>
    public LockMode LockMode { get; set; }

    /// <summary>Provides the <c>LockRetainUntil</c> API.</summary>
    public DateTimeOffset? LockRetainUntil { get; set; }

    /// <summary>Provides the <c>Metadata</c> API.</summary>
    public MetadataBuilder Metadata { get; internal set; }

    /// <summary>Provides the <c>Acl</c> API.</summary>
    public ObjectCannedAcl Acl { get; set; }

    /// <summary>Provides the <c>AclGrantRead</c> API.</summary>
    public AclBuilder AclGrantRead { get; internal set; }

    /// <summary>Provides the <c>AclGrantReadAcp</c> API.</summary>
    public AclBuilder AclGrantReadAcp { get; internal set; }

    /// <summary>Provides the <c>AclGrantWriteAcp</c> API.</summary>
    public AclBuilder AclGrantWriteAcp { get; internal set; }

    /// <summary>Provides the <c>AclGrantFullControl</c> API.</summary>
    public AclBuilder AclGrantFullControl { get; internal set; }

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;

    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    /// <summary>Provides the <c>SseAlgorithm</c> API.</summary>
    public SseAlgorithm SseAlgorithm { get; set; }

    /// <summary>Provides the <c>SseKmsKeyId</c> API.</summary>
    public string? SseKmsKeyId { get; set; }

    /// <summary>Provides the <c>SseContext</c> API.</summary>
    public KmsContextBuilder SseContext { get; internal set; } //setter is needed for fluent Upload

    /// <summary>Provides the <c>SseCustomerAlgorithm</c> API.</summary>
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }

    /// <summary>Provides the <c>SseCustomerKeyMd5</c> API.</summary>
    public byte[]? SseCustomerKeyMd5 { get; set; }

    /// <summary>Provides the <c>SseCustomerKey</c> API.</summary>
    public byte[]? SseCustomerKey
    {
        get => _sseCustomerKey;
        set => _sseCustomerKey = CopyHelper.CopyWithNull(value);
    }

    /// <summary>Provides the <c>ClearSensitiveMaterial</c> API.</summary>
    public void ClearSensitiveMaterial()
    {
        if (_sseCustomerKey != null)
        {
            Array.Clear(_sseCustomerKey, 0, _sseCustomerKey.Length);
            _sseCustomerKey = null;
        }
    }

    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    public StorageClass StorageClass { get; set; }

    /// <summary>Provides the <c>Tags</c> API.</summary>
    public TagBuilder Tags { get; internal set; }

    /// <summary>Provides the <c>WebsiteRedirectLocation</c> API.</summary>
    public string? WebsiteRedirectLocation { get; set; }

    internal void Initialize(string bucketName, string objectKey, Stream? content)
    {
        Content = content;
        BucketName = bucketName;
        ObjectKey = objectKey;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        ClearSensitiveMaterial();

        Tags.Reset();
        Metadata.Reset();
        CacheControl.Reset();
        ContentDisposition.Reset();
        ContentEncoding.Reset();
        ContentType.Reset();
        AclGrantRead.Reset();
        AclGrantReadAcp.Reset();
        AclGrantWriteAcp.Reset();
        AclGrantFullControl.Reset();
        SseContext.Reset();
        ExpiresOn = null;
        LockMode = LockMode.Unknown;
        LockRetainUntil = null;
        LockLegalHold = null;
        Acl = ObjectCannedAcl.Unknown;
        RequestPayer = Payer.Unknown;
        SseAlgorithm = SseAlgorithm.Unknown;
        SseKmsKeyId = null;
        SseCustomerAlgorithm = SseCustomerAlgorithm.Unknown;
        SseCustomerKeyMd5 = null;
        WebsiteRedirectLocation = null;
        StorageClass = StorageClass.Unknown;
        ChecksumAlgorithm = ChecksumAlgorithm.Unknown;
        Checksum = null;

        Content = null;
        ContentMd5 = null;
        base.Reset();
    }
}