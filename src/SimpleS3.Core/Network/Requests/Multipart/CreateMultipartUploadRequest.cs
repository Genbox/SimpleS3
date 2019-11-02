using System;
using System.IO;
using Genbox.HttpBuilders;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Properties;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart
{
    /// <summary>
    /// This operation initiates a multipart upload and returns an upload ID. This upload ID is used to associate all of the parts in the specific
    /// multipart upload. You specify this upload ID in each of your subsequent upload part requests (see Upload Part). You also include this upload ID in
    /// the final request to either complete or abort the multipart upload request.
    /// </summary>
    public class CreateMultipartUploadRequest : BaseRequest, IHasContentProps, IHasCacheControl, IHasStorageClass, IHasLock, IHasObjectAcl, IHasSse, IHasSseCustomerKey, ISupportStreaming
    {
        private byte[] _sseCustomerKey;

        public CreateMultipartUploadRequest(string bucketName, string objectKey) : base(HttpMethod.POST, bucketName, objectKey)
        {
            Tags = new TagBuilder();
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
        /// f the bucket is configured as a website, redirects requests for this object to another object in the same bucket or to an external URL.
        /// Amazon S3 stores the value of this header in the object metadata.
        /// </summary>
        public string WebsiteRedirectLocation { get; set; }

        /// <inheritdoc />
        public CacheControlBuilder CacheControl { get; internal set; }

        /// <inheritdoc />
        public ContentDispositionBuilder ContentDisposition { get; internal set; }

        /// <inheritdoc />
        public ContentEncodingBuilder ContentEncoding { get; internal set; }

        /// <inheritdoc />
        public byte[] ContentMd5 { get; set; }

        /// <inheritdoc />
        public ContentTypeBuilder ContentType { get; internal set; }

        /// <inheritdoc />
        public DateTimeOffset? Expires { get; set; }

        /// <inheritdoc />
        public LockMode LockMode { get; set; }

        /// <inheritdoc />
        public DateTimeOffset? LockRetainUntil { get; set; }

        /// <inheritdoc />
        public bool? LockLegalHold { get; set; }

        /// <inheritdoc />
        public ObjectCannedAcl Acl { get; set; }

        /// <inheritdoc />
        public AclBuilder AclGrantRead { get; internal set; }

        /// <inheritdoc />
        public AclBuilder AclGrantReadAcp { get; internal set; }

        /// <inheritdoc />
        public AclBuilder AclGrantWriteAcp { get; internal set; }

        /// <inheritdoc />
        public AclBuilder AclGrantFullControl { get; internal set; }

        /// <inheritdoc />
        public SseAlgorithm SseAlgorithm { get; set; }

        /// <inheritdoc />
        public string SseKmsKeyId { get; set; }

        /// <inheritdoc />
        public KmsContextBuilder SseContext { get; set; }

        /// <inheritdoc />
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }

        /// <inheritdoc />
        public byte[] SseCustomerKey
        {
            get => _sseCustomerKey;
            set
            {
                if (value == null)
                {
                    _sseCustomerKey = null;
                    return;
                }

                _sseCustomerKey = new byte[value.Length];
                Array.Copy(value, 0, _sseCustomerKey, 0, value.Length);
            }
        }

        /// <inheritdoc />
        public byte[] SseCustomerKeyMd5 { get; set; }

        public void ClearSensitiveMaterial()
        {
            if (_sseCustomerKey != null)
                Array.Clear(_sseCustomerKey, 0, _sseCustomerKey.Length);
        }

        /// <inheritdoc />
        public StorageClass StorageClass { get; set; }
    }
}