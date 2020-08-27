using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genbox.HttpBuilders;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.ErrorHandling.Status;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Fluent
{
    public class Upload
    {
        private readonly IMultipartOperations _multipartOperations;
        private readonly IObjectOperations _objectOperations;
        private readonly PutObjectRequest _request;

        internal Upload(IObjectOperations objectOperations, IMultipartOperations multipartOperations, string bucket, string objectKey)
        {
            _objectOperations = objectOperations;
            _multipartOperations = multipartOperations;

            _request = new PutObjectRequest(bucket, objectKey, null);
        }

        public Upload WithCacheControl(CacheControlBuilder cacheControl)
        {
            _request.CacheControl = cacheControl;
            return this;
        }

        public Upload WithCacheControl(CacheControlType type, int seconds = -1)
        {
            _request.CacheControl.Set(type, seconds);
            return this;
        }

        public Upload WithContentDisposition(ContentDispositionBuilder builder)
        {
            _request.ContentDisposition = builder;
            return this;
        }

        public Upload WithContentDisposition(ContentDispositionType type, string? filename = null)
        {
            _request.ContentDisposition.Set(type, filename);
            return this;
        }

        public Upload WithContentType(ContentTypeBuilder builder)
        {
            _request.ContentType = builder;
            return this;
        }

        public Upload WithContentType(string mediaType, string? charset = null, string? boundary = null)
        {
            _request.ContentType.Set(mediaType, charset, boundary);
            return this;
        }

        public Upload WithContentType(MediaType mediaType, Charset charset = Charset.Utf_8, string? boundary = null)
        {
            _request.ContentType.Set(mediaType, charset, boundary);
            return this;
        }

        public Upload WithContentEncoding(ContentEncodingBuilder builder)
        {
            _request.ContentEncoding = builder;
            return this;
        }

        public Upload WithContentEncoding(ContentEncodingType type)
        {
            _request.ContentEncoding.Add(type);
            return this;
        }

        /// <summary>Enables Server Side Encryption (SSE) using AES. The key is automatically created and maintained on the server.</summary>
        public Upload WithEncryption()
        {
            _request.SseAlgorithm = SseAlgorithm.Aes256;
            return this;
        }

        /// <summary>Enables Server Side Encryption (SSE) with Amazon's Key Management Service (KMS)</summary>
        /// <param name="kmsKeyId">You can use this this specify which KMS master key you want to use.</param>
        /// <param name="kmsContext">Here you can specify the encryption context.</param>
        public Upload WithEncryptionKms(string? kmsKeyId = null, KmsContextBuilder? kmsContext = null)
        {
            _request.SseAlgorithm = SseAlgorithm.AwsKms;

            if (kmsKeyId != null)
                _request.SseKmsKeyId = kmsKeyId;

            if (kmsContext != null)
                _request.SseContext = kmsContext;

            return this;
        }

        /// <summary>Enabled Server Side Encryption (SSE) with the provided key.</summary>
        public Upload WithEncryptionCustomerKey(byte[] encryptionKey)
        {
            _request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            _request.SseCustomerKey = encryptionKey;
            _request.SseCustomerKeyMd5 = CryptoHelper.Md5Hash(encryptionKey);
            return this;
        }

        public Upload WithMetadata(MetadataBuilder metadata)
        {
            _request.Metadata = metadata;
            return this;
        }

        public Upload WithMetadata(string key, string value)
        {
            _request.Metadata.Add(key, value);
            return this;
        }

        public Upload WithStorageClass(StorageClass storageClass)
        {
            _request.StorageClass = storageClass;
            return this;
        }

        public Upload WithTag(TagBuilder tags)
        {
            _request.Tags = tags;
            return this;
        }

        public Upload WithTag(string key, string value)
        {
            _request.Tags.Add(key, value);
            return this;
        }

        public Upload WithAccessControl(ObjectCannedAcl acl)
        {
            _request.Acl = acl;
            return this;
        }

        public Upload WithAccessControl(ObjectAclBuilder acl)
        {
            Validator.RequireNotNull(acl, nameof(acl));

            _request.AclGrantRead = acl.ReadObject;
            _request.AclGrantReadAcp = acl.ReadAcl;
            _request.AclGrantWriteAcp = acl.WriteAcl;
            _request.AclGrantFullControl = acl.FullControl;
            return this;
        }

        public Upload CalculateContentMd5()
        {
            _request.ContentMd5 = _request.Content == null ? Constants.EmptyMd5Bytes : CryptoHelper.Md5Hash(_request.Content, true);
            return this;
        }

        public Upload WithLock(LockMode lockMode, DateTimeOffset retainUntil)
        {
            _request.LockMode = lockMode;
            _request.LockRetainUntil = retainUntil;
            return this;
        }

        public Upload WithLegalHold()
        {
            _request.LockLegalHold = true;
            return this;
        }

        public Upload RemoveLegalHold()
        {
            _request.LockLegalHold = false;
            return this;
        }

        public async Task<MultipartUploadStatus> UploadMultipartAsync(Stream data, CancellationToken token = default)
        {
            _request.Method = HttpMethod.POST;
            _request.Content = null;

            IAsyncEnumerable<UploadPartResponse> async = _multipartOperations.MultipartUploadAsync(_request, data, token: token);

            await foreach (UploadPartResponse resp in async.WithCancellation(token))
            {
                if (!resp.IsSuccess)
                    return MultipartUploadStatus.Incomplete;
            }

            return MultipartUploadStatus.Ok;
        }

        public Task<PutObjectResponse> UploadAsync(Stream data, CancellationToken token = default)
        {
            _request.Method = HttpMethod.PUT;
            _request.Content = data;

            return _objectOperations.PutObjectAsync(_request, token);
        }

        public Task<PutObjectResponse> UploadDataAsync(byte[] data, CancellationToken token = default)
        {
            return UploadAsync(new MemoryStream(data), token);
        }

        public Task<PutObjectResponse> UploadStringAsync(string data, Encoding? encoding = null, CancellationToken token = default)
        {
            if (encoding == null)
                encoding = Constants.Utf8NoBom;

            return UploadDataAsync(encoding.GetBytes(data), token);
        }

        public Upload WithWebsiteRedirectLocation(string url)
        {
            _request.WebsiteRedirectLocation = url;
            return this;
        }
    }
}