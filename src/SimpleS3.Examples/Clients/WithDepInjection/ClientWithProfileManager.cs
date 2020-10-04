using System;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Examples.Clients.WithDepInjection
{
    /// <summary>This example that how to use the ProfileManager extension.</summary>
    public static class ClientWithProfileManager
    {
        public static S3Client Create(string profileName)
        {
            ServiceCollection services = new ServiceCollection();

            //Here we setup our S3Client
            IS3ClientBuilder builder = services.AddSimpleS3();

            //Here we enable in-memory encryption using Microsoft Data Protection
            builder.CoreBuilder
                   .UseProfileManager()
                   .BindConfigToProfile(profileName)
                   .UseConsoleSetup()
                   .UseDataProtection();

            //Finally we build the service provider and return the S3Client
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            //This service initiate a Console based setup process if the profile does not exist
            IProfileSetup setup = serviceProvider.GetRequiredService<IProfileSetup>();
            setup.SetupProfile(profileName);

            return serviceProvider.GetRequiredService<S3Client>();
        }
    }
}