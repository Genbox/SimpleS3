using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Abstracts;

public abstract class ServiceBuilderBase(IServiceCollection services, string? name = null)
{
    public const string DefaultName = "SimpleS3";

    public IServiceCollection Services { get; } = services;

    public string Name { get; } = name ?? DefaultName;

    public static string GetOptionsName(string name) => name == DefaultName ? string.Empty : name;
}