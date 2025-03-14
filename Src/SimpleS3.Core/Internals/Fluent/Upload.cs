﻿using System.Text;
using Genbox.HttpBuilders;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Fluent;

internal sealed class Upload : IUpload
{
    private readonly IMultipartTransfer _multipartTransfer;
    private readonly IObjectOperations _objectOperations;
    private readonly PutObjectRequest _request;

    internal Upload(IObjectOperations objectOperations, IMultipartTransfer multipartTransfer, string bucket, string objectKey)
    {
        _objectOperations = objectOperations;
        _multipartTransfer = multipartTransfer;

        _request = new PutObjectRequest(bucket, objectKey, null);
    }

    public IUpload WithCacheControl(CacheControlBuilder cacheControl)
    {
        _request.CacheControl = cacheControl;
        return this;
    }

    public IUpload WithCacheControl(CacheControlType type, int seconds = -1)
    {
        _request.CacheControl.Add(type, seconds);
        return this;
    }

    public IUpload WithContentDisposition(ContentDispositionBuilder builder)
    {
        _request.ContentDisposition = builder;
        return this;
    }

    public IUpload WithContentDisposition(ContentDispositionType type, string? filename = null)
    {
        _request.ContentDisposition.Set(type, filename);
        return this;
    }

    public IUpload WithContentType(ContentTypeBuilder builder)
    {
        _request.ContentType = builder;
        return this;
    }

    public IUpload WithContentType(string mediaType, string? charset = null, string? boundary = null)
    {
        _request.ContentType.Set(mediaType, charset, boundary);
        return this;
    }

    public IUpload WithContentType(MediaType mediaType, Charset charset = Charset.Utf_8, string? boundary = null)
    {
        _request.ContentType.Set(mediaType, charset, boundary);
        return this;
    }

    public IUpload WithContentEncoding(ContentEncodingBuilder builder)
    {
        _request.ContentEncoding = builder;
        return this;
    }

    public IUpload WithContentEncoding(ContentEncodingType type)
    {
        _request.ContentEncoding.Add(type);
        return this;
    }

    /// <summary>Enables Server Side Encryption (SSE) using AES. The key is automatically created and maintained on the server.</summary>
    public IUpload WithEncryption()
    {
        _request.SseAlgorithm = SseAlgorithm.Aes256;
        return this;
    }

    /// <summary>Enables Server Side Encryption (SSE) with Amazon's Key Management Service (KMS)</summary>
    /// <param name="kmsKeyId">You can use this specify which KMS master key you want to use.</param>
    /// <param name="kmsContext">Here you can specify the encryption context.</param>
    public IUpload WithEncryptionKms(string? kmsKeyId = null, KmsContextBuilder? kmsContext = null)
    {
        _request.SseAlgorithm = SseAlgorithm.AwsKms;

        if (kmsKeyId != null)
            _request.SseKmsKeyId = kmsKeyId;

        if (kmsContext != null)
            _request.SseContext = kmsContext;

        return this;
    }

    /// <summary>Enabled Server Side Encryption (SSE) with the provided key.</summary>
    public IUpload WithEncryptionCustomerKey(byte[] encryptionKey)
    {
        _request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
        _request.SseCustomerKey = encryptionKey;
        _request.SseCustomerKeyMd5 = CryptoHelper.Md5Hash(encryptionKey);
        return this;
    }

    public IUpload WithMetadata(MetadataBuilder metadata)
    {
        _request.Metadata = metadata;
        return this;
    }

    public IUpload WithMetadata(string key, string value)
    {
        _request.Metadata.Add(key, value);
        return this;
    }

    public IUpload WithStorageClass(StorageClass storageClass)
    {
        _request.StorageClass = storageClass;
        return this;
    }

    public IUpload WithTag(TagBuilder tags)
    {
        _request.Tags = tags;
        return this;
    }

    public IUpload WithTag(string key, string value)
    {
        _request.Tags.Add(key, value);
        return this;
    }

    public IUpload WithAccessControl(ObjectCannedAcl acl)
    {
        _request.Acl = acl;
        return this;
    }

    public IUpload WithAccessControl(ObjectAclBuilder acl)
    {
        Validator.RequireNotNull(acl);

        _request.AclGrantRead = acl.ReadObject;
        _request.AclGrantReadAcp = acl.ReadAcl;
        _request.AclGrantWriteAcp = acl.WriteAcl;
        _request.AclGrantFullControl = acl.FullControl;
        return this;
    }

    public IUpload CalculateContentMd5()
    {
        _request.ContentMd5 = _request.Content == null ? Constants.EmptyMd5Bytes : CryptoHelper.Md5Hash(_request.Content, true);
        return this;
    }

    public IUpload WithLock(LockMode lockMode, DateTimeOffset retainUntil)
    {
        _request.LockMode = lockMode;
        _request.LockRetainUntil = retainUntil;
        return this;
    }

    public IUpload WithLegalHold()
    {
        _request.LockLegalHold = true;
        return this;
    }

    public IUpload RemoveLegalHold()
    {
        _request.LockLegalHold = false;
        return this;
    }

    public Task<CompleteMultipartUploadResponse> UploadMultipartAsync(Stream data, CancellationToken token = default)
    {
        //Create a request and copy over all the supported properties
        CreateMultipartUploadRequest req = new CreateMultipartUploadRequest(_request.BucketName, _request.ObjectKey);
        req.CacheControl = _request.CacheControl;
        req.ContentDisposition = _request.ContentDisposition;
        req.ContentType = _request.ContentType;
        req.ContentEncoding = _request.ContentEncoding;
        req.SseAlgorithm = _request.SseAlgorithm;
        req.SseKmsKeyId = _request.SseKmsKeyId;
        req.SseCustomerAlgorithm = _request.SseCustomerAlgorithm;
        req.SseCustomerKey = _request.SseCustomerKey;
        req.SseCustomerKeyMd5 = _request.SseCustomerKeyMd5;
        req.Metadata = _request.Metadata;
        req.StorageClass = _request.StorageClass;
        req.Tags = _request.Tags;
        req.Acl = _request.Acl;
        req.AclGrantRead = _request.AclGrantRead;
        req.AclGrantReadAcp = _request.AclGrantReadAcp;
        req.AclGrantWriteAcp = _request.AclGrantWriteAcp;
        req.AclGrantFullControl = _request.AclGrantFullControl;
        req.LockMode = _request.LockMode;
        req.LockRetainUntil = _request.LockRetainUntil;
        req.LockLegalHold = _request.LockLegalHold;

        return _multipartTransfer.MultipartUploadAsync(req, data, token: token);
    }

    public Task<PutObjectResponse> UploadAsync(Stream? data, CancellationToken token = default)
    {
        _request.Method = HttpMethodType.PUT;
        _request.Content = data;

        return _objectOperations.PutObjectAsync(_request, token);
    }

    public async Task<PutObjectResponse> UploadFileAsync(string filePath, CancellationToken token = default)
    {
        using FileStream fs = File.OpenRead(filePath);
        return await UploadAsync(fs, token).ConfigureAwait(false);
    }

    public async Task<PutObjectResponse> UploadDataAsync(byte[] data, CancellationToken token = default)
    {
        using MemoryStream ms = new MemoryStream(data);
        return await UploadAsync(ms, token).ConfigureAwait(false);
    }

    public Task<PutObjectResponse> UploadStringAsync(string data, Encoding? encoding = null, CancellationToken token = default)
    {
        encoding ??= Constants.Utf8NoBom;

        return UploadDataAsync(encoding.GetBytes(data), token);
    }

    public IUpload WithWebsiteRedirectLocation(string url)
    {
        _request.WebsiteRedirectLocation = url;
        return this;
    }
}