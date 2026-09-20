namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3Redirect</c> API.</summary>
public sealed class S3Redirect
{
    /// <summary>Provides the <c>HostName</c> API.</summary>
    public string? HostName { get; set; }

    /// <summary>Provides the <c>Protocol</c> API.</summary>
    public string? Protocol { get; set; }

    /// <summary>Provides the <c>ReplaceKeyPrefixWith</c> API.</summary>
    public string? ReplaceKeyPrefixWith { get; set; }

    /// <summary>Provides the <c>ReplaceKeyWith</c> API.</summary>
    public string? ReplaceKeyWith { get; set; }

    /// <summary>Provides the <c>HttpRedirectCode</c> API.</summary>
    public string? HttpRedirectCode { get; set; }
}