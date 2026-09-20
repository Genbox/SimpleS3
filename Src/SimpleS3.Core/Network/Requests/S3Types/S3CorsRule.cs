namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3CorsRule</c> API.</summary>
public sealed class S3CorsRule
{
    /// <summary>Provides the <c>Id</c> API.</summary>
    public string? Id { get; set; }

    /// <summary>Provides the <c>AllowedHeaders</c> API.</summary>
    public IList<string> AllowedHeaders { get; } = new List<string>();

    /// <summary>Provides the <c>AllowedMethods</c> API.</summary>
    public IList<string> AllowedMethods { get; } = new List<string>();

    /// <summary>Provides the <c>AllowedOrigins</c> API.</summary>
    public IList<string> AllowedOrigins { get; } = new List<string>();

    /// <summary>Provides the <c>ExposeHeaders</c> API.</summary>
    public IList<string> ExposeHeaders { get; } = new List<string>();

    /// <summary>Provides the <c>MaxAgeSeconds</c> API.</summary>
    public int? MaxAgeSeconds { get; set; }
}