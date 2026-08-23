using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

public sealed class DeleteBucketInventoryConfigurationRequest : BaseRequest, IHasBucketName, IHasExpectedBucketOwner
{
    internal DeleteBucketInventoryConfigurationRequest() : base(HttpMethodType.DELETE) {}
    public DeleteBucketInventoryConfigurationRequest(string bucketName, string configurationId) : this() => Initialize(bucketName, configurationId);
    public string BucketName { get; set; } = null!;
    public string ConfigurationId { get; set; } = null!;
    public string? ExpectedBucketOwner { get; set; }
    internal void Initialize(string bucketName, string configurationId) { BucketName = bucketName; ConfigurationId = configurationId; }
    public override void Reset() { ExpectedBucketOwner = null; base.Reset(); }
}