using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Core.Internals.Builders;

internal sealed partial class QueryParameterAuthorizationBuilder
{
    private readonly ILogger<QueryParameterAuthorizationBuilder> _logger = logger;

    [LoggerMessage(LogLevel.Trace, "Building parameter based authorization")]
    private partial void BuildingParameterAuthorization();
}