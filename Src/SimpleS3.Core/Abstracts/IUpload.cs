using System.Diagnostics.CodeAnalysis;
using System.Text;
using Genbox.HttpBuilders;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Abstracts;

/// <summary>Provides the <c>IUpload</c> API.</summary>
public interface IUpload
{
    /// <summary>Provides the <c>WithCacheControl</c> API.</summary>
    IUpload WithCacheControl(CacheControlBuilder cacheControl);
    /// <summary>Provides the <c>WithCacheControl</c> API.</summary>
    IUpload WithCacheControl(CacheControlType type, int seconds = -1);
    /// <summary>Provides the <c>WithContentDisposition</c> API.</summary>
    IUpload WithContentDisposition(ContentDispositionBuilder builder);
    /// <summary>Provides the <c>WithContentDisposition</c> API.</summary>
    IUpload WithContentDisposition(ContentDispositionType type, string? filename = null);
    /// <summary>Provides the <c>WithContentType</c> API.</summary>
    IUpload WithContentType(ContentTypeBuilder builder);
    /// <summary>Provides the <c>WithContentType</c> API.</summary>
    IUpload WithContentType(string mediaType, string? charset = null, string? boundary = null);
    /// <summary>Provides the <c>WithContentType</c> API.</summary>
    IUpload WithContentType(MediaType mediaType, Charset charset = Charset.Utf_8, string? boundary = null);
    /// <summary>Provides the <c>WithContentEncoding</c> API.</summary>
    IUpload WithContentEncoding(ContentEncodingBuilder builder);
    /// <summary>Provides the <c>WithContentEncoding</c> API.</summary>
    IUpload WithContentEncoding(ContentEncodingType type);

    /// <summary>Enables Server Side Encryption (SSE) using AES. The key is automatically created and maintained on the server.</summary>
    IUpload WithEncryption();

    /// <summary>Enables Server Side Encryption (SSE) with Amazon's Key Management Service (KMS)</summary>
    /// <param name="kmsKeyId">You can use this specify which KMS master key you want to use.</param>
    /// <param name="kmsContext">Here you can specify the encryption context.</param>
    IUpload WithEncryptionKms(string? kmsKeyId = null, KmsContextBuilder? kmsContext = null);

    /// <summary>Enabled Server Side Encryption (SSE) with the provided key.</summary>
    IUpload WithEncryptionCustomerKey(byte[] encryptionKey);

    /// <summary>Provides the <c>WithMetadata</c> API.</summary>
    IUpload WithMetadata(MetadataBuilder metadata);
    /// <summary>Provides the <c>WithMetadata</c> API.</summary>
    IUpload WithMetadata(string key, string value);
    /// <summary>Provides the <c>WithStorageClass</c> API.</summary>
    IUpload WithStorageClass(StorageClass storageClass);
    /// <summary>Provides the <c>WithTag</c> API.</summary>
    IUpload WithTag(TagBuilder tags);
    /// <summary>Provides the <c>WithTag</c> API.</summary>
    IUpload WithTag(string key, string value);
    /// <summary>Provides the <c>WithAccessControl</c> API.</summary>
    IUpload WithAccessControl(ObjectCannedAcl acl);
    /// <summary>Provides the <c>WithAccessControl</c> API.</summary>
    IUpload WithAccessControl(ObjectAclBuilder acl);
    /// <summary>Provides the <c>CalculateContentMd5</c> API.</summary>
    IUpload CalculateContentMd5();
    /// <summary>Provides the <c>WithLock</c> API.</summary>
    IUpload WithLock(LockMode lockMode, DateTimeOffset retainUntil);
    /// <summary>Provides the <c>WithLegalHold</c> API.</summary>
    IUpload WithLegalHold();
    /// <summary>Provides the <c>RemoveLegalHold</c> API.</summary>
    IUpload RemoveLegalHold();

    /// <summary>Provides the <c>UploadMultipartAsync</c> API.</summary>
    Task<CompleteMultipartUploadResponse> UploadMultipartAsync(Stream data, CancellationToken token = default);
    /// <summary>Provides the <c>UploadAsync</c> API.</summary>
    Task<PutObjectResponse> UploadAsync(Stream? data, CancellationToken token = default);
    /// <summary>Provides the <c>UploadFileAsync</c> API.</summary>
    Task<PutObjectResponse> UploadFileAsync(string filePath, CancellationToken token = default);
    /// <summary>Provides the <c>UploadDataAsync</c> API.</summary>
    Task<PutObjectResponse> UploadDataAsync(byte[] data, CancellationToken token = default);
    /// <summary>Provides the <c>UploadStringAsync</c> API.</summary>
    Task<PutObjectResponse> UploadStringAsync(string data, Encoding? encoding = null, CancellationToken token = default);
    /// <summary>Provides the <c>WithWebsiteRedirectLocation</c> API.</summary>
    [SuppressMessage("Design", "CA1054", Justification = "S3 website redirects accept object-key paths as well as absolute URLs.")]
    IUpload WithWebsiteRedirectLocation(string url);
}