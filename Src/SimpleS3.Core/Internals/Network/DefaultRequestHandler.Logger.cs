using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Core.Internals.Network;

internal sealed partial class DefaultRequestHandler
{
    private readonly ILogger<DefaultRequestHandler> _logger;

    [LoggerMessage(LogLevel.Trace, "Handling {RequestType} with request id {RequestId}")]
    private partial void HandlingRequest(string requestType, Guid requestId);

    [LoggerMessage(LogLevel.Debug, "ContentSha256 is {ContentSha256}")]
    private partial void ContentSha256(string contentSha256);
}