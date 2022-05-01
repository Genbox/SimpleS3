using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Internals;

internal class CoreBuilder : ServiceBuilderBase, ICoreBuilder
{
    public CoreBuilder(IServiceCollection services, string? name = null) : base(services, name) {}
}