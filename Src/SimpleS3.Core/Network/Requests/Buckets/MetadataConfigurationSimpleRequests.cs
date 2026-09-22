using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Base request for an S3 Metadata subresource without a body.</summary>
public abstract class MetadataConfigurationSimpleRequest : BaseRequest, IHasBucketName
{
    /// <summary>Creates a metadata request using the specified HTTP method.</summary>
    protected MetadataConfigurationSimpleRequest(HttpMethodType method) : base(method) {}

    /// <summary>Bucket name.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName) => BucketName = bucketName;

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        BucketName = null!;
        base.Reset();
    }
}

/// <summary>Deletes an S3 Metadata V2 configuration.</summary>
public sealed class DeleteBucketMetadataConfigurationRequest : MetadataConfigurationSimpleRequest
{
    internal DeleteBucketMetadataConfigurationRequest() : base(HttpMethodType.DELETE) {}

    /// <summary>Creates a request.</summary>
    public DeleteBucketMetadataConfigurationRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }
}

/// <summary>Gets an S3 Metadata V2 configuration.</summary>
public sealed class GetBucketMetadataConfigurationRequest : MetadataConfigurationSimpleRequest
{
    internal GetBucketMetadataConfigurationRequest() : base(HttpMethodType.GET) {}

    /// <summary>Creates a request.</summary>
    public GetBucketMetadataConfigurationRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }
}