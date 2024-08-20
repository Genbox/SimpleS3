using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Internals;

internal sealed class CoreBuilder(IServiceCollection services, string? name = null) : ServiceBuilderBase(services, name), ICoreBuilder;