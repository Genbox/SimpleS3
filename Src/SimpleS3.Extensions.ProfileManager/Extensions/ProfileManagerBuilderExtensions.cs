using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Internal.Setup;
using Microsoft.Extensions.DependencyInjection;
#if COMMERCIAL
using Genbox.SimpleS3.Extensions.ProfileManager.Internal.DataProtection;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Microsoft.AspNetCore.DataProtection;
#endif

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions;

public static class ProfileManagerBuilderExtensions
{
#if COMMERCIAL
    public static IDataProtectionBuilder UseDataProtection(this IProfileManagerBuilder builder)
    {
        IDataProtectionBuilder dataProtectionBuilder = builder.Services.AddDataProtection(options => options.ApplicationDiscriminator = "SimpleS3");
        builder.Services.AddSingleton<IAccessKeyProtector, DataProtectionKeyProtector>();
        return dataProtectionBuilder;
    }
#endif

    public static IProfileManagerBuilder BindConfigToProfile(this IProfileManagerBuilder builder, string profileName)
    {
        builder.Services.Configure<SimpleS3Config>((x, y) => x.UseProfile(y.GetRequiredService<IProfileManager>(), profileName));
        return builder;
    }

    public static IProfileManagerBuilder BindConfigToDefaultProfile(this IProfileManagerBuilder builder)
    {
        builder.Services.Configure<SimpleS3Config>((x, y) => x.UseDefaultProfile(y.GetRequiredService<IProfileManager>()));
        return builder;
    }

    public static IProfileManagerBuilder UseConsoleSetup(this IProfileManagerBuilder builder)
    {
        builder.Services.AddSingleton<ConsoleProfileSetup>();
        builder.Services.AddSingleton<IRegionConverter, RegionConverter>();
        return builder;
    }
}