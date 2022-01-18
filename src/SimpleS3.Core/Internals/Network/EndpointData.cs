namespace Genbox.SimpleS3.Core.Internals.Network;

public class EndpointData : IEndpointData
{
    private readonly int _hashCode;

    public EndpointData(string host, string endpoint)
    {
        Host = host;
        Endpoint = endpoint;

        _hashCode = Endpoint.GetHashCode();
    }

    public string Host { get; }
    public string Endpoint { get; }

    public override int GetHashCode()
    {
        return _hashCode;
    }
}