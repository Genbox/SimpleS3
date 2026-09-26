using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Base request for S3 Metadata bodies.</summary>
public abstract class MetadataConfigurationBodyRequest : BaseRequest, IHasBucketName, IContentMd5Config, IHasChecksumProperties
{
    /// <summary>Creates a metadata request using the specified HTTP method.</summary>
    protected MetadataConfigurationBodyRequest(HttpMethodType method) : base(method) {}

    /// <summary>Content MD5.</summary>
    public byte[]? ContentMd5 { get; set; }

    /// <summary>Whether the content MD5 header is required.</summary>
    public Func<bool> ForceContentMd5 => () => true;

    /// <summary>Bucket name.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Checksum algorithm.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; set; }

    /// <summary>Checksum.</summary>
    public byte[]? Checksum { get; set; }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    internal override void Reset()
    {
        ContentMd5 = null;
        ChecksumAlgorithm = ChecksumAlgorithm.Unknown;
        Checksum = null;
        BucketName = null!;
        base.Reset();
    }
}

/// <summary>Creates an S3 Metadata V2 configuration.</summary>
public sealed class CreateBucketMetadataConfigurationRequest : MetadataConfigurationBodyRequest
{
    internal CreateBucketMetadataConfigurationRequest() : base(HttpMethodType.POST) {}

    /// <summary>Creates a request.</summary>
    public CreateBucketMetadataConfigurationRequest(string bucketName, S3MetadataConfiguration configuration) : this()
    {
        Initialize(bucketName, configuration);
    }

    /// <summary>Configuration.</summary>
    public S3MetadataConfiguration Configuration { get; private set; } = null!;

    internal void Initialize(string bucketName, S3MetadataConfiguration configuration)
    {
        BucketName = bucketName;
        Configuration = configuration;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    internal override void Reset()
    {
        Configuration = null!;
        base.Reset();
    }
}

/// <summary>Updates a live inventory table configuration.</summary>
public sealed class UpdateBucketMetadataInventoryTableConfigurationRequest : MetadataConfigurationBodyRequest
{
    internal UpdateBucketMetadataInventoryTableConfigurationRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Creates a request.</summary>
    public UpdateBucketMetadataInventoryTableConfigurationRequest(string bucketName, S3InventoryTableConfiguration configuration) : this()
    {
        Initialize(bucketName, configuration);
    }

    /// <summary>Configuration.</summary>
    public S3InventoryTableConfiguration Configuration { get; private set; } = null!;

    internal void Initialize(string bucketName, S3InventoryTableConfiguration configuration)
    {
        BucketName = bucketName;
        Configuration = configuration;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    internal override void Reset()
    {
        Configuration = null!;
        base.Reset();
    }
}

/// <summary>Updates a journal table configuration.</summary>
public sealed class UpdateBucketMetadataJournalTableConfigurationRequest : MetadataConfigurationBodyRequest
{
    internal UpdateBucketMetadataJournalTableConfigurationRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Creates a request.</summary>
    public UpdateBucketMetadataJournalTableConfigurationRequest(string bucketName, S3JournalTableConfiguration configuration) : this()
    {
        Initialize(bucketName, configuration);
    }

    /// <summary>Configuration.</summary>
    public S3JournalTableConfiguration Configuration { get; private set; } = null!;

    internal void Initialize(string bucketName, S3JournalTableConfiguration configuration)
    {
        BucketName = bucketName;
        Configuration = configuration;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    internal override void Reset()
    {
        Configuration = null!;
        base.Reset();
    }
}