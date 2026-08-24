using System.Net;
using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory;

public partial class HttpClientFactoryNetworkDriver
{
    private readonly ILogger<HttpClientFactoryNetworkDriver> _logger = logger;

    [LoggerMessage(LogLevel.Trace, "Sending HTTP request")]
    private partial void SendingRequest();

    [LoggerMessage(LogLevel.Debug, "Got an {Status} response with {StatusCode}")]
    private partial void ReceivedResponse(string status, HttpStatusCode statusCode);
}