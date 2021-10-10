using System;
using System.Collections.Generic;
using System.Net;
using Genbox.SimpleS3.AmazonS3.Extensions;
using Genbox.SimpleS3.BackBlazeB2.Extensions;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Genbox.SimpleS3.GoogleCloudStorage.Extensions;
using Genbox.SimpleS3.ProviderBase.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.ProviderTests
{
    public class ProviderSetup
    {
        private ProviderSetup()
        {
            BuildProvider(S3Provider.AmazonS3);
            BuildProvider(S3Provider.BackBlazeB2);
            BuildProvider(S3Provider.GoogleCloudStorage);
        }

        public static ProviderSetup Instance { get; } = new ProviderSetup();
        public IList<(S3Provider, IProfile, ISimpleClient)> Clients { get; } = new List<(S3Provider, IProfile, ISimpleClient)>();

        private void BuildProvider(S3Provider provider)
        {
            ServiceCollection services = new ServiceCollection();
            IClientBuilder clientBuilder;

            switch (provider)
            {
                case S3Provider.AmazonS3:
                    clientBuilder = services.AddAmazonS3();
                    break;
                case S3Provider.BackBlazeB2:
                    clientBuilder = services.AddBackBlazeB2();
                    break;
                case S3Provider.GoogleCloudStorage:
                    clientBuilder = services.AddGoogleCloudStorage();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(provider), provider, null);
            }

            clientBuilder
                .CoreBuilder
                .UseProfileManager()
                .BindConfigToProfile("TestSetup-" + provider);

            clientBuilder.HttpBuilder.UseProxy(new WebProxy("http://127.0.0.1:8888"));

            ServiceProvider p = services.BuildServiceProvider();

            IProfileManager profileManager = p.GetRequiredService<IProfileManager>();

            IProfile? profile = profileManager.GetProfile("TestSetup-" + provider);

            if (profile == null)
                throw new InvalidOperationException("Unable to find profile");

            Clients.Add((provider, profile, p.GetRequiredService<ISimpleClient>()));
        }
    }
}