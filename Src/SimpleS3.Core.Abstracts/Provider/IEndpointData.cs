namespace Genbox.SimpleS3.Core.Abstracts.Provider;

public interface IEndpointData
{
    string Host { get; }
    string Endpoint { get; }
}