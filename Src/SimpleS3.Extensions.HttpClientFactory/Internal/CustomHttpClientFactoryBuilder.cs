using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Internal;

internal sealed class CustomHttpClientFactoryBuilder(IServiceCollection services, string? name = null) : ServiceBuilderBase(services, name), IHttpClientBuilder;