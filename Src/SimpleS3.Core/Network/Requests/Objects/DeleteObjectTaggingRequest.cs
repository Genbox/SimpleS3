using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

public sealed class DeleteObjectTaggingRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer
{
    internal DeleteObjectTaggingRequest() : base(HttpMethodType.DELETE) {}

    public DeleteObjectTaggingRequest(string bucketName, string objectKey) : this()
    {
        Initialize(bucketName, objectKey);
    }

    public string BucketName { get; set; } = null!;
    public string ObjectKey { get; set; } = null!;
    public Payer RequestPayer { get; set; }
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
    }

    public override void Reset()
    {
        RequestPayer = Payer.Unknown;
        VersionId = null;
        base.Reset();
    }
}