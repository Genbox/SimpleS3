using Genbox.SimpleS3.Core.Abstracts.Provider;

namespace Genbox.SimpleS3.Core.Internals.Network;

internal class EndpointData : IEndpointData
{
    public EndpointData(string host, string? bucket, string regionCode, string endpoint)
    {
        Host = host;
        Bucket = bucket;
        RegionCode = regionCode;
        Endpoint = endpoint;
    }

    public string? Bucket { get; }
    public string RegionCode { get; }
    public string Host { get; }
    public string Endpoint { get; }
}