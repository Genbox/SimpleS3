namespace Genbox.SimpleS3.Core.Network.Responses.Buckets
{
    /// <summary>
    /// GetBucketAccelerateConfiguration
    /// </summary>
    public class GetBucketAccelerateConfigurationResponse : BaseResponse
    {
        public bool AccelerateEnabled { get; internal set; }
    }
}