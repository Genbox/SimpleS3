using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Core.Internals.Network;

internal sealed partial class DefaultSignedRequestHandler
{
    private readonly ILogger<DefaultSignedRequestHandler> _logger;

    [LoggerMessage(LogLevel.Trace, "Handling {RequestType} with request id {RequestId}")]
    private partial void HandlingRequest(string requestType, Guid requestId);
}