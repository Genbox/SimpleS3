using Genbox.SimpleS3.Core.Abstracts.Provider;

namespace Genbox.SimpleS3.Core.Internals.Network;

internal class EndpointData(string host, string? bucket, string regionCode, string endpoint) : IEndpointData
{
    public string? Bucket { get; } = bucket;
    public string RegionCode { get; } = regionCode;
    public string Host { get; } = host;
    public string Endpoint { get; } = endpoint;
}