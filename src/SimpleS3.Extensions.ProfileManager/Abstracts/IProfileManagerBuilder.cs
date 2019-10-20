using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts
{
    public interface IProfileManagerBuilder
    {
        IServiceCollection Services { get; }
    }
}