using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Fluent;
using Genbox.SimpleS3.Core.Misc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests
{
    public abstract class OfflineTestBase : IDisposable
    {
        private readonly IConfigurationRoot _configRoot;

        protected OfflineTestBase(ITestOutputHelper outputHelper)
        {
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("Config.json", false);

            ServiceCollection collection = new ServiceCollection();

            //Set the configuration from the config file
            _configRoot = configBuilder.Build();

            collection.AddSimpleS3Core(ConfigureS3);
            collection.AddSingleton<INetworkDriver, NullNetworkDriver>();

            collection.AddLogging(x =>
            {
                x.AddConfiguration(_configRoot.GetSection("Logging"));
                x.AddXUnit(outputHelper);
            });

            Services = collection.BuildServiceProvider();

            BucketName = _configRoot["BucketName"] ?? "main-test-bucket-2019";

            Config = Services.GetRequiredService<IOptions<S3Config>>().Value;
            ObjectClient = Services.GetRequiredService<IObjectClient>();
            BucketClient = Services.GetRequiredService<IBucketClient>();
            MultipartClient = Services.GetRequiredService<IMultipartClient>();
            Transfer = Services.GetRequiredService<Transfer>();
        }

        public ServiceProvider Services { get; }

        protected S3Config Config { get; }
        protected string BucketName { get; }
        protected IObjectClient ObjectClient { get; }
        protected IBucketClient BucketClient { get; }
        protected IMultipartClient MultipartClient { get; }
        protected Transfer Transfer { get; }

        public void Dispose()
        {
            Services?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void ConfigureS3(S3Config config)
        {
            //Set the configuration from the config file
            _configRoot.Bind(config);

            config.Credentials = new StringAccessKey("keyidkeyidkeyidkeyid", "accesskeyacceskey123accesskeyacceskey123");
        }
    }
}
