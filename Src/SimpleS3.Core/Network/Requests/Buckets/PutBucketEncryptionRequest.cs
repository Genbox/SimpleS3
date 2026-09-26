using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Configures default encryption and blocked encryption types for an existing bucket.</summary>
public class PutBucketEncryptionRequest : BaseRequest, IHasBucketName, IContentMd5Config
{
    internal PutBucketEncryptionRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutBucketEncryptionRequest</c> API.</summary>
    [SuppressMessage("Design", "CA1062", Justification = "Initialize consumes the required collection before construction completes; preserve its existing null-input failure behavior.")]
    public PutBucketEncryptionRequest(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules) : this()
    {
        Initialize(bucketName, rules);
    }

    /// <summary>Provides the <c>Rules</c> API.</summary>
    public IList<S3ServerSideEncryptionRule> Rules { get; } = new List<S3ServerSideEncryptionRule>();

    /// <summary>Provides the <c>ForceContentMd5</c> API.</summary>
    public Func<bool> ForceContentMd5 => () => true;

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules)
    {
        BucketName = bucketName;

        foreach (S3ServerSideEncryptionRule rule in rules)
            Rules.Add(rule);
    }

    internal override void Reset()
    {
        ContentMd5 = null;
        Rules.Clear();

        BucketName = null!;
        base.Reset();
    }
}