using System;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>
/// This implementation of the PUT operation adds an object to a bucket. You must have WRITE permissions on a bucket to add an object to it.
/// Amazon S3 never adds partial objects; if you receive a success response, Amazon S3 added the entire object to the bucket.
/// </summary>
public sealed class PutObjectRequest : CreateMultipartUploadRequest, IHasContent, ISupportStreaming, IContentMd5Config
{
    internal PutObjectRequest() : base(HttpMethodType.PUT) { }

    public PutObjectRequest(string bucketName, string objectKey, Stream? content) : this()
    {
        Initialize(bucketName, objectKey, content);
    }

    public byte[]? ContentMd5 { get; set; }
    Func<bool> IContentMd5Config.ForceContentMd5 => () => LockLegalHold.HasValue && LockLegalHold.Value || LockMode != LockMode.Unknown;

    public Stream? Content { get; internal set; }

    internal void Initialize(string bucketName, string objectKey, Stream? content)
    {
        Content = content;
        Initialize(bucketName, objectKey);
    }

    public override void Reset()
    {
        Content = null;
        ContentMd5 = null;

        base.Reset();
    }
}