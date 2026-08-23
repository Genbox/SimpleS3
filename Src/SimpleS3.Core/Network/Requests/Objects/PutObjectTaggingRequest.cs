using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

public sealed class PutObjectTaggingRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IContentMd5Config
{
    internal PutObjectTaggingRequest() : base(HttpMethodType.PUT) {}

    public PutObjectTaggingRequest(string bucketName, string objectKey, IDictionary<string, string> tags) : this()
    {
        Initialize(bucketName, objectKey, tags);
    }

    public string BucketName { get; set; } = null!;
    public byte[]? ContentMd5 { get; set; }
    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;
    public string ObjectKey { get; set; } = null!;
    public Payer RequestPayer { get; set; }
    public IDictionary<string, string> Tags { get; private set; } = new Dictionary<string, string>();
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey, IDictionary<string, string> tags)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        Tags = tags;
    }

    public override void Reset()
    {
        ContentMd5 = null;
        RequestPayer = Payer.Unknown;
        Tags = new Dictionary<string, string>();
        VersionId = null;
        base.Reset();
    }
}