namespace Genbox.SimpleS3.Core.Internals.Network;

public interface IEndpointData
{
    string Host { get; }
    string Endpoint { get; }
}