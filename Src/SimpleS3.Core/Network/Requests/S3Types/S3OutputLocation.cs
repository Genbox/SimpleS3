using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public class S3OutputLocation(string bucketName, string prefix) : IHasSse, IHasObjectAcl, IHasStorageClass, IHasBucketName
{
    /// <summary>
    /// Header starting with this prefix are user-defined metadata. Each one is stored and returned as a set of key-value pairs. Amazon S3 doesn't validate or interpret
    /// user-defined metadata.
    /// </summary>
    public MetadataBuilder Metadata { get; } = new MetadataBuilder();

    /// <summary>
    /// Specifies a set of one or more tags to associate with the object. These tags are stored in the tagging subresource that is associated with the object. To specify tags on
    /// an object, the requester must have s3:PutObjectTagging included in the list of permitted actions in their IAM policy.
    /// </summary>
    public TagBuilder Tags { get; } = new TagBuilder();

    /// <summary>The prefix that is prepended to the restore results for this request.</summary>
    public string? Prefix { get; private set; } = prefix;

    /// <summary>The name of the bucket where the restore results will be placed.</summary>
    public string BucketName { get; set; } = bucketName;

    public ObjectCannedAcl Acl { get; set; }
    public AclBuilder AclGrantRead { get; } = new AclBuilder();
    public AclBuilder AclGrantReadAcp { get; } = new AclBuilder();
    public AclBuilder AclGrantWriteAcp { get; } = new AclBuilder();
    public AclBuilder AclGrantFullControl { get; } = new AclBuilder();
    public SseAlgorithm SseAlgorithm { get; set; }
    public string? SseKmsKeyId { get; set; }
    public KmsContextBuilder SseContext { get; } = new KmsContextBuilder();
    public StorageClass StorageClass { get; set; }

    internal void Reset()
    {
        Metadata.Reset();
        Tags.Reset();
        Prefix = null;
        Acl = ObjectCannedAcl.Unknown;
        AclGrantRead.Reset();
        AclGrantReadAcp.Reset();
        AclGrantWriteAcp.Reset();
        AclGrantFullControl.Reset();
        SseAlgorithm = SseAlgorithm.Unknown;
        SseKmsKeyId = null;
        SseContext.Reset();
        StorageClass = StorageClass.Unknown;
    }
}