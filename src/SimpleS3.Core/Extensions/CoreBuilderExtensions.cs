using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Region;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Extensions
{
    public static class CoreBuilderExtensions
    {
        public static ICoreBuilder UseAwsS3(this ICoreBuilder builder)
        {
            builder.Services.AddSingleton<IRegionManager>(x => RegionManagers.BuildAws());
            return builder;
        }
    }
}