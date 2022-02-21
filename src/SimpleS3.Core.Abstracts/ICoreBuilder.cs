using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Abstracts;

public interface ICoreBuilder
{
    IServiceCollection Services { get; }

    string Name { get; }
}