using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Core.Internals.Network;

public partial class DefaultResponseHandler
{
    private readonly ILogger<DefaultResponseHandler> _logger;

    [LoggerMessage(LogLevel.Debug, "Sending request to {Url}")]
    private partial void SendingRequest(string url);

    [LoggerMessage(LogLevel.Debug, "Received error: '{Message}'. Details: '{Details}'")]
    private partial void ReceivedError(string message, string details);

    [LoggerMessage(LogLevel.Debug, "Failed to map error")]
    private partial void FailedToMapError(Exception exception);
}