using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClient.Internal;

internal sealed class CustomHttpClientBuilder(IServiceCollection services, string? name = null) : ServiceBuilderBase(services, name), IHttpClientBuilder;