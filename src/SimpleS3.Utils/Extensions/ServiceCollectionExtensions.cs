using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Utils.Extensions
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        //The extensions here add the ability to get the IServiceProvider when binding options

        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, Action<TOptions, IServiceProvider> configureOptions) where TOptions : class
        {
            return services.Configure(Options.DefaultName, configureOptions);
        }

        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, Action<TOptions, IServiceProvider> configureOptions) where TOptions : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            services.AddOptions();
            services.AddSingleton<IConfigureOptions<TOptions>>(x => new ConfigureNamedOptions<TOptions, IServiceProvider>(name, x, configureOptions));
            return services;
        }

        public static IServiceCollection PostConfigure<TOptions>(this IServiceCollection services, Action<TOptions, IServiceProvider> configureOptions) where TOptions : class
        {
            return services.PostConfigure(Options.DefaultName, configureOptions);
        }

        public static IServiceCollection PostConfigure<TOptions>(this IServiceCollection services, string name, Action<TOptions, IServiceProvider> configureOptions) where TOptions : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            services.AddOptions();
            services.AddSingleton<IPostConfigureOptions<TOptions>>(x => new PostConfigureOptions<TOptions, IServiceProvider>(name, x, configureOptions));
            return services;
        }
    }
}