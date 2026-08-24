namespace Genbox.SimpleS3.Core.Abstracts.Provider;

/// <summary>Provides the <c>IEndpointData</c> API.</summary>
public interface IEndpointData
{
    /// <summary>Provides the <c>Host</c> API.</summary>
    string Host { get; }
    /// <summary>Provides the <c>Endpoint</c> API.</summary>
    string Endpoint { get; }
}