using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>UpdateBucketMetadataAnnotationTableConfigurationRequest</c> API.</summary>
public sealed class UpdateBucketMetadataAnnotationTableConfigurationRequest : BaseRequest, IHasBucketName, IContentMd5Config, IHasChecksumProperties
{
    internal UpdateBucketMetadataAnnotationTableConfigurationRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>UpdateBucketMetadataAnnotationTableConfigurationRequest</c> API.</summary>
    public UpdateBucketMetadataAnnotationTableConfigurationRequest(string bucketName, S3AnnotationTableConfiguration configuration) : this()
    {
        Initialize(bucketName, configuration);
    }

    /// <summary>Provides the <c>Configuration</c> API.</summary>
    public S3AnnotationTableConfiguration Configuration { get; private set; } = null!;

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; set; }

    /// <summary>Provides the <c>Checksum</c> API.</summary>
    public byte[]? Checksum { get; set; }

    internal void Initialize(string bucketName, S3AnnotationTableConfiguration configuration)
    {
        BucketName = bucketName;
        Configuration = configuration;
    }

    internal override void Reset()
    {
        Configuration = null!;
        ContentMd5 = null;
        ChecksumAlgorithm = ChecksumAlgorithm.Unknown;
        Checksum = null;
        BucketName = null!;
        base.Reset();
    }
}