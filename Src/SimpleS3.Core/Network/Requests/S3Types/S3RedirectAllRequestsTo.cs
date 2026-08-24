namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3RedirectAllRequestsTo</c> API.</summary>
public sealed class S3RedirectAllRequestsTo(string hostName)
{
    /// <summary>Provides the <c>HostName</c> API.</summary>
    public string HostName { get; set; } = hostName;
    /// <summary>Provides the <c>Protocol</c> API.</summary>
    public string? Protocol { get; set; }
}