using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genbox.HttpBuilders;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

#if COMMERCIAL
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
#endif

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IUpload
    {
        IUpload WithCacheControl(CacheControlBuilder cacheControl);
        IUpload WithCacheControl(CacheControlType type, int seconds = -1);
        IUpload WithContentDisposition(ContentDispositionBuilder builder);
        IUpload WithContentDisposition(ContentDispositionType type, string? filename = null);
        IUpload WithContentType(ContentTypeBuilder builder);
        IUpload WithContentType(string mediaType, string? charset = null, string? boundary = null);
        IUpload WithContentType(MediaType mediaType, Charset charset = Charset.Utf_8, string? boundary = null);
        IUpload WithContentEncoding(ContentEncodingBuilder builder);
        IUpload WithContentEncoding(ContentEncodingType type);

        /// <summary>Enables Server Side Encryption (SSE) using AES. The key is automatically created and maintained on the server.</summary>
        IUpload WithEncryption();

        /// <summary>Enables Server Side Encryption (SSE) with Amazon's Key Management Service (KMS)</summary>
        /// <param name="kmsKeyId">You can use this specify which KMS master key you want to use.</param>
        /// <param name="kmsContext">Here you can specify the encryption context.</param>
        IUpload WithEncryptionKms(string? kmsKeyId = null, KmsContextBuilder? kmsContext = null);

        /// <summary>Enabled Server Side Encryption (SSE) with the provided key.</summary>
        IUpload WithEncryptionCustomerKey(byte[] encryptionKey);

        IUpload WithMetadata(MetadataBuilder metadata);
        IUpload WithMetadata(string key, string value);
        IUpload WithStorageClass(StorageClass storageClass);
        IUpload WithTag(TagBuilder tags);
        IUpload WithTag(string key, string value);
        IUpload WithAccessControl(ObjectCannedAcl acl);
        IUpload WithAccessControl(ObjectAclBuilder acl);
        IUpload CalculateContentMd5();
        IUpload WithLock(LockMode lockMode, DateTimeOffset retainUntil);
        IUpload WithLegalHold();
        IUpload RemoveLegalHold();

#if COMMERCIAL
        Task<CompleteMultipartUploadResponse> UploadMultipartAsync(Stream data, CancellationToken token = default);
#endif

        Task<PutObjectResponse> UploadAsync(Stream? data, CancellationToken token = default);
        Task<PutObjectResponse> UploadDataAsync(byte[] data, CancellationToken token = default);
        Task<PutObjectResponse> UploadStringAsync(string data, Encoding? encoding = null, CancellationToken token = default);
        IUpload WithWebsiteRedirectLocation(string url);
    }
}