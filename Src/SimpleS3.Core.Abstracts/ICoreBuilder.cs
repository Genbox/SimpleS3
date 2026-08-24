using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Abstracts;

/// <summary>Provides the <c>ICoreBuilder</c> API.</summary>
public interface ICoreBuilder
{
    /// <summary>Provides the <c>Services</c> API.</summary>
    IServiceCollection Services { get; }

    /// <summary>Provides the <c>Name</c> API.</summary>
    string Name { get; }
}