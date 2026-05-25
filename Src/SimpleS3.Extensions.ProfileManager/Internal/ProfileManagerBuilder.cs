using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Internal;

internal class ProfileManagerBuilder(IServiceCollection services, string name) : IProfileManagerBuilder
{
    public IServiceCollection Services { get; } = services;
    public string Name { get; } = name;
}