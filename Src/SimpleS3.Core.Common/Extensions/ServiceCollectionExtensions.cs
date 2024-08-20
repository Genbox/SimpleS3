using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Common.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>Same as Microsoft.DependencyInjection's TryAddEnumerable, but also returns if it was able to add it or not</summary>
    public static bool TryAddEnumerableRet(this IServiceCollection services, ServiceDescriptor descriptor)
    {
        Type? implType = descriptor.ImplementationType;

        if (implType == typeof(object) || implType == descriptor.ServiceType)
            throw new ArgumentException($"Implementation type {implType} is the same as {descriptor.ServiceType}", nameof(descriptor));

        int count = services.Count;
        for (int i = 0; i < count; i++)
        {
            ServiceDescriptor service = services[i];

            if (service.ServiceType == descriptor.ServiceType && service.ImplementationType == implType)
                return false; // Already added
        }

        services.Add(descriptor);
        return true;
    }

    //The extensions here add the ability to get the IServiceProvider when binding options

    public static IServiceCollection Configure<TOptions>(this IServiceCollection services, Action<TOptions, IServiceProvider> configureOptions) where TOptions : class => services.Configure(Options.DefaultName, configureOptions);

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

    public static IServiceCollection PostConfigure<TOptions>(this IServiceCollection services, Action<TOptions, IServiceProvider> configureOptions) where TOptions : class => services.PostConfigure(Options.DefaultName, configureOptions);

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