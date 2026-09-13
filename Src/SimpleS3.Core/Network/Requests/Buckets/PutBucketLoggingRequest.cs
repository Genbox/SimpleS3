using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>PutBucketLoggingRequest</c> API.</summary>
public sealed class PutBucketLoggingRequest : BaseRequest, IHasBucketName, IContentMd5Config
{
    internal PutBucketLoggingRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutBucketLoggingRequest</c> API.</summary>
    public PutBucketLoggingRequest(string bucketName, S3LoggingConfiguration? loggingConfiguration) : this()
    {
        Initialize(bucketName, loggingConfiguration);
    }

    /// <summary>Provides the <c>LoggingConfiguration</c> API.</summary>
    public S3LoggingConfiguration? LoggingConfiguration { get; private set; }

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName, S3LoggingConfiguration? loggingConfiguration)
    {
        BucketName = bucketName;
        LoggingConfiguration = loggingConfiguration;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        ContentMd5 = null;
        LoggingConfiguration = null;
        BucketName = null!;
        base.Reset();
    }
}