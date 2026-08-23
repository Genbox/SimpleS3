namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public sealed class S3RedirectAllRequestsTo(string hostName)
{
    public string HostName { get; set; } = hostName;
    public string? Protocol { get; set; }
}