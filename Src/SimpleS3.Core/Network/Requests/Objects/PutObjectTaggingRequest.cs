using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Provides the <c>PutObjectTaggingRequest</c> API.</summary>
public sealed class PutObjectTaggingRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IContentMd5Config
{
    internal PutObjectTaggingRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutObjectTaggingRequest</c> API.</summary>
    public PutObjectTaggingRequest(string bucketName, string objectKey, IDictionary<string, string> tags) : this()
    {
        Initialize(bucketName, objectKey, tags);
    }

    /// <summary>Provides the <c>Tags</c> API.</summary>
    public IDictionary<string, string> Tags { get; private set; } = null!;

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;

    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey, IDictionary<string, string> tags)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        Tags = tags;
    }

    internal override void Reset()
    {
        ContentMd5 = null;
        RequestPayer = Payer.Unknown;
        Tags = null!;
        VersionId = null;
        BucketName = null!;
        ObjectKey = null!;
        base.Reset();
    }
}