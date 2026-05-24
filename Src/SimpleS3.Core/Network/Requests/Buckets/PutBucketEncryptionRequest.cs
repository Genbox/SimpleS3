using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Configures default encryption and blocked encryption types for an existing bucket.</summary>
public class PutBucketEncryptionRequest : BaseRequest, IHasBucketName, IContentMd5Config
{
    internal PutBucketEncryptionRequest() : base(HttpMethodType.PUT) {}

    public PutBucketEncryptionRequest(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules) : this()
    {
        Initialize(bucketName, rules);
    }

    public IList<S3ServerSideEncryptionRule> Rules { get; } = new List<S3ServerSideEncryptionRule>();
    public Func<bool> ForceContentMd5 => () => true;
    public byte[]? ContentMd5 { get; set; }
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules)
    {
        BucketName = bucketName;

        foreach (S3ServerSideEncryptionRule rule in rules)
            Rules.Add(rule);
    }

    public override void Reset()
    {
        ContentMd5 = null;
        Rules.Clear();

        base.Reset();
    }
}