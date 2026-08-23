namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public sealed class S3WebsiteConfiguration
{
    public string? IndexDocumentSuffix { get; set; }
    public string? ErrorDocumentKey { get; set; }
    public S3RedirectAllRequestsTo? RedirectAllRequestsTo { get; set; }
    public IList<S3RoutingRule> RoutingRules { get; } = new List<S3RoutingRule>();
}