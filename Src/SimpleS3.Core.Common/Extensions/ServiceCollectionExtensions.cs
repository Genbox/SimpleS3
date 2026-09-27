using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Common.Extensions;

/// <summary>Provides the <c>ServiceCollectionExtensions</c> API.</summary>
public static class ServiceCollectionExtensions
{
    //The extensions here add the ability to get the IServiceProvider when binding options

    /// <summary>Provides the <c>Configure&lt;TOptions&gt;</c> API.</summary>
    public static IServiceCollection Configure<TOptions>(this IServiceCollection services, Action<TOptions, IServiceProvider> configureOptions) where TOptions : class => services.Configure(Options.DefaultName, configureOptions);

    /// <summary>Provides the <c>Configure&lt;TOptions&gt;</c> API.</summary>
    public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, Action<TOptions, IServiceProvider> configureOptions) where TOptions : class
    {
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        services.AddOptions();
        services.AddSingleton<IConfigureOptions<TOptions>>(x => new ConfigureNamedOptions<TOptions, IServiceProvider>(name, x, configureOptions));
        return services;
    }

    /// <summary>Provides the <c>PostConfigure&lt;TOptions&gt;</c> API.</summary>
    public static IServiceCollection PostConfigure<TOptions>(this IServiceCollection services, Action<TOptions, IServiceProvider> configureOptions) where TOptions : class => services.PostConfigure(Options.DefaultName, configureOptions);

    /// <summary>Provides the <c>PostConfigure&lt;TOptions&gt;</c> API.</summary>
    public static IServiceCollection PostConfigure<TOptions>(this IServiceCollection services, string name, Action<TOptions, IServiceProvider> configureOptions) where TOptions : class
    {
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        services.AddOptions();
        services.AddSingleton<IPostConfigureOptions<TOptions>>(x => new PostConfigureOptions<TOptions, IServiceProvider>(name, x, configureOptions));
        return services;
    }
}