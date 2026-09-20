using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Creates a new lifecycle configuration for the bucket or replaces an existing lifecycle configuration.</summary>
public class PutBucketLifecycleConfigurationRequest : BaseRequest, IHasBucketName, IContentMd5Config
{
    internal PutBucketLifecycleConfigurationRequest() : base(HttpMethodType.PUT)
    {
        Rules = new List<S3Rule>();
    }

    /// <summary>Provides the <c>PutBucketLifecycleConfigurationRequest</c> API.</summary>
    [SuppressMessage("Design", "CA1062", Justification = "Initialize consumes the required collection before construction completes; preserve its existing null-input failure behavior.")]
    public PutBucketLifecycleConfigurationRequest(string bucketName, IEnumerable<S3Rule> rules) : this()
    {
        Initialize(bucketName, rules);
    }

    /// <summary>Provides the <c>Rules</c> API.</summary>
    public IList<S3Rule> Rules { get; }

    /// <summary>Provides the <c>ForceContentMd5</c> API.</summary>
    public Func<bool> ForceContentMd5 => () => true;

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName, IEnumerable<S3Rule> rules)
    {
        BucketName = bucketName;

        foreach (S3Rule s3Rule in rules)
            Rules.Add(s3Rule);
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        ContentMd5 = null;
        Rules.Clear();

        base.Reset();
    }
}