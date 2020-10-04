using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Utility.TestSetup
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            IConfigurationRoot configRoot = new ConfigurationBuilder()
                                      .AddJsonFile("Config.json", false)
                                      .Build();

            ServiceCollection services = new ServiceCollection();

            services.Configure<AwsConfig>(configRoot);

            IS3ClientBuilder clientBuilder = services.AddSimpleS3();

            string profileName = configRoot["ProfileName"];

            clientBuilder.CoreBuilder.UseProfileManager()
                         .BindConfigToProfile(profileName)
                         .UseConsoleSetup()
                         .UseDataProtection();

            IConfigurationSection proxySection = configRoot.GetSection("Proxy");

            if (proxySection != null && proxySection["UseProxy"].Equals("true", StringComparison.OrdinalIgnoreCase))
                clientBuilder.HttpBuilder.WithProxy(proxySection["ProxyAddress"]);

            using (ServiceProvider provider = services.BuildServiceProvider())
            {
                IProfileSetup setup = provider.GetRequiredService<IProfileSetup>();
                setup.SetupProfile(profileName);

                IBucketClient bucketClient = provider.GetRequiredService<IBucketClient>();

                string bucketName = configRoot["BucketName"];

                Console.WriteLine($"Setting up bucket '{bucketName}'");

                HeadBucketResponse headResp = await bucketClient.HeadBucketAsync(bucketName).ConfigureAwait(false);

                if (headResp.StatusCode == 200)
                    Console.WriteLine($"'{bucketName}' already exist.");
                else if (headResp.StatusCode == 404)
                {
                    Console.WriteLine($"'{bucketName}' does not exist - creating.");
                    CreateBucketResponse createResp = await bucketClient.CreateBucketAsync(bucketName, x => x.EnableObjectLocking = true).ConfigureAwait(false);

                    if (createResp.IsSuccess)
                        Console.WriteLine($"Successfully created '{bucketName}'.");
                    else
                    {
                        Console.Error.WriteLine($"Failed to create '{bucketName}'. Exiting.");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"Unknown error code when checking if bucket exists: {headResp.StatusCode}");
                    return;
                }

                Console.WriteLine("Adding lock configuration");

                PutBucketLockConfigurationResponse putLockResp = await bucketClient.PutBucketLockConfigurationAsync(bucketName, true).ConfigureAwait(false);

                if (putLockResp.IsSuccess)
                    Console.WriteLine("Successfully applied lock configuration.");
                else
                    Console.Error.WriteLine("Failed to apply lock configuration.");

                List<S3Rule> rules = new List<S3Rule>
                {
                    new S3Rule("ExpireAll", true)
                    {
                        AbortIncompleteMultipartUploadDays = 1,
                        NonCurrentVersionExpirationDays = 1,
                        Expiration = new S3Expiration(1),
                        Filter = new S3Filter { Prefix = "" }
                    }
                };

                Console.WriteLine("Adding lifecycle configuration");

                PutBucketLifecycleConfigurationResponse putLifecycleResp = await bucketClient.PutBucketLifecycleConfigurationAsync(bucketName, rules).ConfigureAwait(false);

                if (putLifecycleResp.IsSuccess)
                    Console.WriteLine("Successfully applied lifecycle configuration.");
                else
                    Console.Error.WriteLine("Failed to apply lifecycle configuration.");
            }
        }
    }
}