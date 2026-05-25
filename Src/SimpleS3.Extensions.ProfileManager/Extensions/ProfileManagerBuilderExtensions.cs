using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Internal;
using Genbox.SimpleS3.Extensions.ProfileManager.Internal.DataProtection;
using Genbox.SimpleS3.Extensions.ProfileManager.Setup;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions;

public static class ProfileManagerBuilderExtensions
{
    public static IDataProtectionBuilder UseDataProtection(this IProfileManagerBuilder builder)
    {
        IDataProtectionBuilder dataProtectionBuilder = builder.Services.AddDataProtection(options => options.ApplicationDiscriminator = "SimpleS3");
        builder.Services.TryAddSingleton<IAccessKeyProtector, DataProtectionKeyProtector>();
        return dataProtectionBuilder;
    }

    public static IProfileManagerBuilder BindConfigToProfile(this IProfileManagerBuilder builder, string profileName)
    {
        builder.Services.Configure<SimpleS3Config>(GetOptionsName(builder), (x, y) => x.UseProfile(y.GetRequiredService<IProfileManager>(), profileName));
        return builder;
    }

    public static IProfileManagerBuilder BindConfigToDefaultProfile(this IProfileManagerBuilder builder)
    {
        builder.Services.Configure<SimpleS3Config>(GetOptionsName(builder), (x, y) => x.UseDefaultProfile(y.GetRequiredService<IProfileManager>()));
        return builder;
    }

    public static IProfileManagerBuilder UseConsoleSetup(this IProfileManagerBuilder builder)
    {
        string serviceName = GetServiceName(builder);

        builder.Services.AddKeyedSingleton<IProfileSetup>(serviceName, (provider, _) => CreateConsoleProfileSetup(provider, serviceName));
        builder.Services.AddSingleton<IProfileSetup>(provider => CreateConsoleProfileSetup(provider, serviceName));
        builder.Services.AddSingleton<IRegionConverter, RegionConverter>();
        return builder;
    }

    private static string GetOptionsName(IProfileManagerBuilder builder) => builder is ProfileManagerBuilder profileBuilder ? profileBuilder.Name : string.Empty;

    private static string GetServiceName(IProfileManagerBuilder builder)
    {
        string optionsName = GetOptionsName(builder);
        return optionsName.Length == 0 ? ServiceBuilderBase.DefaultName : optionsName;
    }

    private static ConsoleProfileSetup CreateConsoleProfileSetup(IServiceProvider provider, string serviceName)
    {
        IRegionData regionData = GetRegionData(provider, serviceName);
        IInputValidator inputValidator = GetInputValidator(provider, serviceName);
        return new ConsoleProfileSetup(provider.GetRequiredService<IProfileManager>(), inputValidator, new RegionConverter(regionData), regionData);
    }

    private static IRegionData GetRegionData(IServiceProvider provider, string serviceName)
    {
        if (serviceName == ServiceBuilderBase.DefaultName)
            return provider.GetService<IRegionData>() ?? provider.GetRequiredKeyedService<IRegionData>(serviceName);

        return provider.GetRequiredKeyedService<IRegionData>(serviceName);
    }

    private static IInputValidator GetInputValidator(IServiceProvider provider, string serviceName)
    {
        if (serviceName == ServiceBuilderBase.DefaultName)
            return provider.GetService<IInputValidator>() ?? provider.GetRequiredKeyedService<IInputValidator>(serviceName);

        return provider.GetRequiredKeyedService<IInputValidator>(serviceName);
    }
}