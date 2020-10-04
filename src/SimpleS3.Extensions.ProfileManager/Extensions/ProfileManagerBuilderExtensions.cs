using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.DataProtection;
using Genbox.SimpleS3.Extensions.ProfileManager.Setup;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Extensions
{
    public static class ProfileManagerBuilderExtensions
    {
        public static IDataProtectionBuilder UseDataProtection(this IProfileManagerBuilder builder)
        {
            IDataProtectionBuilder dataProtectionBuilder = builder.Services.AddDataProtection(options => options.ApplicationDiscriminator = "SimpleS3");
            builder.Services.AddSingleton<IAccessKeyProtector, DataProtectionKeyProtector>();
            return dataProtectionBuilder;
        }

        public static IProfileManagerBuilder BindConfigToProfile(this IProfileManagerBuilder builder, string profileName)
        {
            builder.Services.PostConfigure<Config>((x, y) => x.UseProfile(y.GetRequiredService<IProfileManager>(), profileName));
            return builder;
        }

        public static IProfileManagerBuilder BindConfigToDefaultProfile(this IProfileManagerBuilder builder)
        {
            builder.Services.PostConfigure<Config>((x, y) => x.UseDefaultProfile(y.GetRequiredService<IProfileManager>()));
            return builder;
        }

        public static IProfileManagerBuilder UseConsoleSetup(this IProfileManagerBuilder builder)
        {
            builder.Services.AddSingleton<ConsoleSetup>();
            return builder;
        }
    }
}