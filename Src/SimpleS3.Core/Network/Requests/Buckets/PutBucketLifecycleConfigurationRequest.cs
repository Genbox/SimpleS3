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

    public PutBucketLifecycleConfigurationRequest(string bucketName, IEnumerable<S3Rule> rules) : this()
    {
        Initialize(bucketName, rules);
    }

    public IList<S3Rule> Rules { get; }
    public Func<bool> ForceContentMd5 => () => true;
    public byte[]? ContentMd5 { get; set; }

    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName, IEnumerable<S3Rule> rules)
    {
        BucketName = bucketName;

        foreach (S3Rule s3Rule in rules)
            Rules.Add(s3Rule);
    }

    public override void Reset()
    {
        ContentMd5 = null;
        Rules.Clear();

        base.Reset();
    }
}