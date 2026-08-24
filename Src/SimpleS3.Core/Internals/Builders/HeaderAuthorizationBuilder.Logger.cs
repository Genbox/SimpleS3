using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Core.Internals.Builders;

internal sealed partial class HeaderAuthorizationBuilder
{
    private readonly ILogger<HeaderAuthorizationBuilder> _logger = logger;

    [LoggerMessage(LogLevel.Trace, "Building header based authorization")]
    private partial void BuildingHeaderAuthorization();

    [LoggerMessage(LogLevel.Debug, "AuthHeader: {AuthHeader}")]
    private partial void LogAuthorizationHeader(string authHeader);

    [LoggerMessage(LogLevel.Debug, "Authorization header created")]
    private partial void AuthorizationHeaderCreated();
}