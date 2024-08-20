using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Abstracts;

public abstract class ServiceBuilderBase(IServiceCollection services, string? name = null)
{
    public static string DefaultName => string.Empty;

    public IServiceCollection Services { get; } = services;

    public string Name { get; } = name ?? DefaultName;
}