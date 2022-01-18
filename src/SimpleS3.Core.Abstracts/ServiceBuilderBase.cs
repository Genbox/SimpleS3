using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Abstracts;

public abstract class ServiceBuilderBase
{
    protected ServiceBuilderBase(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}