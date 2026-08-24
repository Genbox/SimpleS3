using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Abstracts;

/// <summary>Provides the <c>ServiceBuilderBase</c> API.</summary>
public abstract class ServiceBuilderBase(IServiceCollection services, string? name = null)
{
    /// <summary>Provides the <c>DefaultName</c> API.</summary>
    public static string DefaultName => string.Empty;

    /// <summary>Provides the <c>Services</c> API.</summary>
    public IServiceCollection Services { get; } = services;

    /// <summary>Provides the <c>Name</c> API.</summary>
    public string Name { get; } = name ?? DefaultName;
}