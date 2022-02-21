using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Abstracts;

public abstract class ServiceBuilderBase
{
    public static string DefaultName => "SimpleS3";

    protected ServiceBuilderBase(IServiceCollection services, string? name = null)
    {
        Name = name ?? DefaultName;
        Services = services;
    }

    public IServiceCollection Services { get; }

    public string Name { get; }
}