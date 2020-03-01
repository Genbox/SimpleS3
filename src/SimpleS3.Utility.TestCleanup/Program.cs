using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.ErrorHandling.Status;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Utility.TestCleanup
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IConfigurationRoot root = new ConfigurationBuilder()
                .AddJsonFile("Config.json", false)
                .Build();

            ServiceCollection services = new ServiceCollection();

            services.Configure<S3Config>(root);

            IS3ClientBuilder clientBuilder = services.AddSimpleS3((s3Config, provider) => root.Bind(s3Config));

            clientBuilder.CoreBuilder.UseProfileManager()
                                     .BindConfigToDefaultProfile()
                                     .UseDataProtection();

            IConfigurationSection proxySection = root.GetSection("Proxy");

            if (proxySection != null && proxySection["UseProxy"].Equals("true", StringComparison.OrdinalIgnoreCase))
                clientBuilder.HttpBuilder.WithProxy(proxySection["ProxyAddress"]);

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                IProfileManager manager = serviceProvider.GetRequiredService<IProfileManager>();
                IProfile profile = manager.GetDefaultProfile();

                //If profile is null, then we do not yet have a profile stored on disk. We use ConsoleSetup as an easy and secure way of asking for credentials
                if (profile == null)
                    ConsoleSetup.SetupDefaultProfile(manager);

                S3Client client = serviceProvider.GetRequiredService<S3Client>();

                await foreach (S3Bucket bucket in client.ListAllBucketsAsync())
                {
                    if (!bucket.Name.StartsWith("testbucket-", StringComparison.OrdinalIgnoreCase))
                        continue;

                    DeleteAllObjectsStatus objDelResp = await client.DeleteAllObjectsAsync(bucket.Name).ConfigureAwait(false);

                    if (objDelResp == DeleteAllObjectsStatus.Ok)
                        await client.DeleteBucketAsync(bucket.Name).ConfigureAwait(false);
                }

                //Empty the main test bucket
                await client.DeleteAllObjectsAsync(root["BucketName"]).ConfigureAwait(false);
            }
        }
    }
}