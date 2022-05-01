using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Abstracts;

public abstract class ServiceBuilderBase
{
    public static string DefaultName => string.Empty;

    protected ServiceBuilderBase(IServiceCollection services, string? name = null)
    {
        Name = name ?? DefaultName;
        Services = services;
    }

    public IServiceCollection Services { get; }

    public string Name { get; }
}