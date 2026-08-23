using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

public sealed class UpdateBucketMetadataAnnotationTableConfigurationRequest : BaseRequest, IHasBucketName, IContentMd5Config, IHasChecksumProperties
{
    internal UpdateBucketMetadataAnnotationTableConfigurationRequest() : base(HttpMethodType.PUT) {}
    public UpdateBucketMetadataAnnotationTableConfigurationRequest(string bucketName, S3AnnotationTableConfiguration configuration) : this() => Initialize(bucketName, configuration);
    public string BucketName { get; set; } = null!;
    public S3AnnotationTableConfiguration Configuration { get; private set; } = null!;
    public byte[]? ContentMd5 { get; set; }
    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;
    public ChecksumAlgorithm ChecksumAlgorithm { get; set; }
    public byte[]? Checksum { get; set; }

    internal void Initialize(string bucketName, S3AnnotationTableConfiguration configuration)
    {
        BucketName = bucketName;
        Configuration = configuration;
    }

    public override void Reset()
    {
        Configuration = null!;
        ContentMd5 = null;
        ChecksumAlgorithm = ChecksumAlgorithm.Unknown;
        Checksum = null;
        base.Reset();
    }
}