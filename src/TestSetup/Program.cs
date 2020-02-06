using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Setup;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.TestSetup
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IConfigurationRoot root = new ConfigurationBuilder()
                .AddJsonFile("Config.json", false)
                .Build();

            ServiceCollection services = new ServiceCollection();

            IClientBuilder s3Builder = services.AddSimpleS3Core();

            s3Builder.UseProfileManager()
                .BindConfigToDefaultProfile()
                .UseDataProtection();

            s3Builder.UseHttpClientFactory().WithProxy("http://127.0.0.1:8888");

            using (ServiceProvider provider = services.BuildServiceProvider())
            {
                IProfileManager manager = provider.GetRequiredService<IProfileManager>();
                IProfile profile = manager.GetDefaultProfile();

                //If profile is null, then we do not yet have a profile stored on disk. We use ConsoleSetup as an easy and secure way of asking for credentials
                if (profile == null)
                {
                    Console.WriteLine("No profile found. Starting setup.");
                    ConsoleSetup.SetupDefaultProfile(manager);
                }

                IBucketClient bucketClient = provider.GetRequiredService<IBucketClient>();

                string bucketName = root["BucketName"];

                Console.WriteLine($"Setting up bucket '{bucketName}'");

                HeadBucketResponse headResp = await bucketClient.HeadBucketAsync(bucketName).ConfigureAwait(false);

                if (headResp.IsSuccess)
                    Console.WriteLine($"'{bucketName}' already exist.");
                else
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

                Console.WriteLine("Adding lock configuration");

                PutBucketLockConfigurationResponse putLockResp = await bucketClient.PutBucketLockConfigurationAsync(bucketName, true).ConfigureAwait(false);

                if (putLockResp.IsSuccess)
                    Console.WriteLine("Successfully applied lock configuration.");
                else
                    Console.Error.WriteLine("Failed to apply lock configuration.");

                List<S3Rule> rules = new List<S3Rule>
                {
                    new S3Rule(true, "ExpireAll")
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