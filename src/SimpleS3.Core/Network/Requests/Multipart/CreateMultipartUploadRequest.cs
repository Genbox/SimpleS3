using System;
using Genbox.HttpBuilders;
using Genbox.SimpleS3.Abstracts.Enums;
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
    public class CreateMultipartUploadRequest : BaseRequest, IHasContentProps, IHasExpiresOn, IHasCacheControl, IHasStorageClass, IHasLock, IHasObjectAcl, IHasSse, IHasSseCustomerKey, IHasRequestPayer, IHasBucketName, IHasObjectKey, IHasWebsireRedirect, IHasMetadata, IHasTags
    {
        private byte[] _sseCustomerKey;

        public CreateMultipartUploadRequest(string bucketName, string objectKey) : base(HttpMethod.POST)
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
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

        public string BucketName { get; set; }
        public CacheControlBuilder CacheControl { get; internal set; }
        public ContentDispositionBuilder ContentDisposition { get; internal set; }
        public ContentEncodingBuilder ContentEncoding { get; internal set; }
        public ContentTypeBuilder ContentType { get; internal set; }
        public DateTimeOffset? ExpiresOn { get; set; }
        public LockMode LockMode { get; set; }
        public DateTimeOffset? LockRetainUntil { get; set; }
        public bool? LockLegalHold { get; set; }

        public MetadataBuilder Metadata { get; internal set; }
        public ObjectCannedAcl Acl { get; set; }
        public AclBuilder AclGrantRead { get; internal set; }
        public AclBuilder AclGrantReadAcp { get; internal set; }
        public AclBuilder AclGrantWriteAcp { get; internal set; }
        public AclBuilder AclGrantFullControl { get; internal set; }
        public string ObjectKey { get; set; }
        public Payer RequestPayer { get; set; }
        public SseAlgorithm SseAlgorithm { get; set; }
        public string SseKmsKeyId { get; set; }
        public KmsContextBuilder SseContext { get; set; }
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }
        public byte[] SseCustomerKeyMd5 { get; set; }

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

        public void ClearSensitiveMaterial()
        {
            if (_sseCustomerKey != null)
                Array.Clear(_sseCustomerKey, 0, _sseCustomerKey.Length);
        }

        public StorageClass StorageClass { get; set; }
        public TagBuilder Tags { get; internal set; }
        public string WebsiteRedirectLocation { get; set; }
    }
}