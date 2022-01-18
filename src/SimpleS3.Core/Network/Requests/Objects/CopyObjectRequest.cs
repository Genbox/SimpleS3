using System;
using Genbox.HttpBuilders;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>
/// Creates a copy of an object that is already stored in Amazon S3. When copying an object, you can preserve all metadata (default) or specify
/// new metadata. However, the ACL is not preserved and is set to private for the user making the request. To override the default ACL setting, specify a
/// new ACL when generating a copy request.
/// </summary>
public class CopyObjectRequest : BaseRequest, IHasObjectAcl, IHasCache, IHasMetadata, IHasTags, IHasLock, IHasSse, IHasSseCustomerKey, IHasStorageClass, IHasRequestPayer, IHasWebsiteRedirect, IHasVersionId, IHasBucketName, IHasObjectKey, IHasLegalHold
{
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
        Tags = new TagBuilder();
        SseContext = new KmsContextBuilder();
    }

    public CopyObjectRequest(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey) : this()
    {
        Initialize(sourceBucketName, sourceObjectKey, destinationBucketName, destinationObjectKey);
    }

    public string SourceBucketName { get; private set; }
    public string SourceObjectKey { get; private set; }
    public string DestinationBucketName { get; private set; }
    public string DestinationObjectKey { get; private set; }
    public MetadataDirective MetadataDirective { get; set; }
    public TaggingDirective TaggingDirective { get; set; }
    string IHasBucketName.BucketName => DestinationBucketName;
    public DateTimeOffset? IfModifiedSince { get; set; }
    public DateTimeOffset? IfUnmodifiedSince { get; set; }
    public ETagBuilder IfETagMatch { get; }
    public ETagBuilder IfETagNotMatch { get; }
    public bool? LockLegalHold { get; set; }
    public LockMode LockMode { get; set; }
    public DateTimeOffset? LockRetainUntil { get; set; }
    public MetadataBuilder Metadata { get; }
    public ObjectCannedAcl Acl { get; set; }
    public AclBuilder AclGrantRead { get; }
    public AclBuilder AclGrantReadAcp { get; }
    public AclBuilder AclGrantWriteAcp { get; }
    public AclBuilder AclGrantFullControl { get; }
    string IHasObjectKey.ObjectKey => DestinationObjectKey;
    public Payer RequestPayer { get; set; }
    public SseAlgorithm SseAlgorithm { get; set; }
    public string? SseKmsKeyId { get; set; }
    public KmsContextBuilder SseContext { get; }
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }

    public byte[]? SseCustomerKey
    {
        get => _sseCustomerKey;
        set => _sseCustomerKey = CopyHelper.CopyWithNull(value);
    }

    public void ClearSensitiveMaterial()
    {
        if (_sseCustomerKey != null)
        {
            Array.Clear(_sseCustomerKey, 0, _sseCustomerKey.Length);
            _sseCustomerKey = null;
        }
    }

    public byte[]? SseCustomerKeyMd5 { get; set; }
    public StorageClass StorageClass { get; set; }
    public TagBuilder Tags { get; }
    public string? VersionId { get; set; }
    public string? WebsiteRedirectLocation { get; set; }

    internal void Initialize(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey)
    {
        SourceBucketName = sourceBucketName;
        SourceObjectKey = sourceObjectKey;
        DestinationBucketName = destinationBucketName;
        DestinationObjectKey = destinationObjectKey;
    }

    public override void Reset()
    {
        ClearSensitiveMaterial();

        MetadataDirective = MetadataDirective.Copy;
        TaggingDirective = TaggingDirective.Copy;
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
        SseKmsKeyId = null;
        SseContext.Reset();
        SseCustomerAlgorithm = SseCustomerAlgorithm.Unknown;
        SseCustomerKeyMd5 = null;
        StorageClass = StorageClass.Unknown;
        Tags.Reset();
        VersionId = null;
        WebsiteRedirectLocation = null;

        base.Reset();
    }
}