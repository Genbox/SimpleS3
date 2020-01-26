using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Examples.Clients.DependencyInjection
{
    /// <summary>This example that how to use the ProfileManager extension.</summary>
    public static class AmazonDiWithProfileManager
    {
        public static S3Client Create()
        {
            ServiceCollection services = new ServiceCollection();

            //Here we setup our S3Client
            IClientBuilder builder = services.AddSimpleS3();

            //Here we enable in-memory encryption using Microsoft Data Protection
            builder.UseProfileManager()
                .BindConfigToProfile("MyProfile")
                .UseDataProtection();

            //Finally we build the service provider and return the S3Client
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IProfileManager manager = serviceProvider.GetRequiredService<IProfileManager>();
            IProfile profile = manager.GetProfile("MyProfile");

            //If profile is null, then we do not yet have a profile stored on disk. We use ConsoleSetup as an easy and secure way of asking for credentials
            if (profile == null)
                ConsoleSetup.SetupProfile(manager, "MyProfile");

            return serviceProvider.GetRequiredService<S3Client>();
        }
    }
}