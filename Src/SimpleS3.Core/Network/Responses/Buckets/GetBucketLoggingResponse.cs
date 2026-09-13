using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketLoggingResponse</c> API.</summary>
public sealed class GetBucketLoggingResponse : BaseResponse
{
    /// <summary>Gets the logging configuration, or <see langword="null" /> when logging is disabled.</summary>
    public S3LoggingConfiguration? LoggingConfiguration { get; internal set; }
}