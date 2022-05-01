using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Abstracts;

public abstract class ServiceBuilderBase
{
    protected ServiceBuilderBase(IServiceCollection services, string? name = null)
    {
        Name = name ?? DefaultName;
        Services = services;
    }

    public static string DefaultName => string.Empty;

    public IServiceCollection Services { get; }

    public string Name { get; }
}