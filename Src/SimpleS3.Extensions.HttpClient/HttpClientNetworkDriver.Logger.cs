using System.Net;
using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Extensions.HttpClient;

public sealed partial class HttpClientNetworkDriver
{
    [LoggerMessage(LogLevel.Trace, "Sending HTTP request")]
    private partial void SendingRequest();

    [LoggerMessage(LogLevel.Debug, "Got an {Status} response with {StatusCode}")]
    private partial void ReceivedResponse(string status, HttpStatusCode statusCode);
}