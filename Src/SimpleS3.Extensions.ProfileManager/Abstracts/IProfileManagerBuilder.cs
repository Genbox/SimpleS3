using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

/// <summary>Provides the <c>IProfileManagerBuilder</c> API.</summary>
public interface IProfileManagerBuilder
{
    /// <summary>Provides the <c>Services</c> API.</summary>
    IServiceCollection Services { get; }
}