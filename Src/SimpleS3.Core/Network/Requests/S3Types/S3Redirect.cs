namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public sealed class S3Redirect
{
    public string? HostName { get; set; }
    public string? Protocol { get; set; }
    public string? ReplaceKeyPrefixWith { get; set; }
    public string? ReplaceKeyWith { get; set; }
    public string? HttpRedirectCode { get; set; }
}