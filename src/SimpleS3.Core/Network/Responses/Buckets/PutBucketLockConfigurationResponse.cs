using Genbox.SimpleS3.Core.Network.Responses.Properties;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets
{
    public class PutBucketLockConfigurationResponse : BaseResponse, IHasRequestCharged
    {
        public bool RequestCharged { get; internal set; }
    }
}