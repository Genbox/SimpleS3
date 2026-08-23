namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public sealed class S3CorsRule
{
    public string? Id { get; set; }
    public IList<string> AllowedHeaders { get; } = new List<string>();
    public IList<string> AllowedMethods { get; } = new List<string>();
    public IList<string> AllowedOrigins { get; } = new List<string>();
    public IList<string> ExposeHeaders { get; } = new List<string>();
    public int? MaxAgeSeconds { get; set; }
}