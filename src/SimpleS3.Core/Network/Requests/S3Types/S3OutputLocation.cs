using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Properties;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    public class S3OutputLocation : OutputLocation, IHasSse, IHasObjectAcl, IHasStorageClass
    {
        public S3OutputLocation(string bucketName, string prefix)
        {
            BucketName = bucketName;
            Prefix = prefix;

            AclGrantRead = new AclBuilder();
            AclGrantReadAcp = new AclBuilder();
            AclGrantWriteAcp = new AclBuilder();
            AclGrantFullControl = new AclBuilder();

            Tags = new TagBuilder();
            Metadata = new MetadataBuilder();
        }

        /// <summary>
        /// Header starting with this prefix are user-defined metadata. Each one is stored and returned as a set of key-value pairs. Amazon S3 doesn't
        /// validate or interpret user-defined metadata.
        /// </summary>
        public MetadataBuilder Metadata { get; internal set; }

        /// <summary>
        /// Specifies a set of one or more tags to associate with the object. These tags are stored in the tagging subresource that is associated with
        /// the object. To specify tags on an object, the requester must have s3:PutObjectTagging included in the list of permitted actions in their IAM policy.
        /// </summary>
        public TagBuilder Tags { get; internal set; }

        /// <summary>
        /// The name of the bucket where the restore results will be placed.
        /// </summary>
        public string BucketName { get; }

        /// <summary>
        /// The prefix that is prepended to the restore results for this request.
        /// </summary>
        public string Prefix { get; }

        public ObjectCannedAcl Acl { get; set; }
        public AclBuilder AclGrantRead { get; internal set; }
        public AclBuilder AclGrantReadAcp { get; internal set; }
        public AclBuilder AclGrantWriteAcp { get; internal set; }
        public AclBuilder AclGrantFullControl { get; internal set; }
        public SseAlgorithm SseAlgorithm { get; set; }
        public string SseKmsKeyId { get; set; }
        public KmsContextBuilder SseContext { get; set; }
        public StorageClass StorageClass { get; set; }
    }
}