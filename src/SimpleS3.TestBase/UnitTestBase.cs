using System;
using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.TestBase
{
    public abstract class UnitTestBase : IDisposable
    {
        protected UnitTestBase(ITestOutputHelper outputHelper)
        {
            IConfigurationRoot configRoot = new ConfigurationBuilder()
                                            .AddJsonFile("Config.json", false)
                                            .Build();

            ServiceCollection collection = new ServiceCollection();
            collection.AddSingleton(configRoot);
            collection.Configure<Config>(configRoot);
            collection.Configure<Config>(ConfigureConfig);

            ICoreBuilder coreBuilder = collection.AddSimpleS3Core();

            ConfigureCoreBuilder(coreBuilder, configRoot);

            collection.AddLogging(x =>
            {
                x.AddConfiguration(configRoot.GetSection("Logging"));
                x.AddXUnit(outputHelper);
            });

            //A small hack to remove all validators, as we test them separately
            collection.RemoveAll(typeof(IValidator<>));
            collection.Replace(ServiceDescriptor.Singleton<IInputValidator, NullInputValidator>());

            ConfigureServices(collection);

            Services = collection.BuildServiceProvider();

            ObjectClient = Services.GetRequiredService<IObjectClient>();
            BucketClient = Services.GetRequiredService<IBucketClient>();
            MultipartClient = Services.GetRequiredService<IMultipartClient>();
            SignedObjectClient = Services.GetRequiredService<ISignedObjectClient>();
            Transfer = Services.GetRequiredService<Transfer>();
        }

        protected ServiceProvider Services { get; }
        protected string BucketName { get; set; }
        protected IObjectClient ObjectClient { get; }
        protected IBucketClient BucketClient { get; }
        protected IMultipartClient MultipartClient { get; }
        protected ISignedObjectClient SignedObjectClient { get; }
        protected Transfer Transfer { get; }

        public virtual void Dispose()
        {
            Services?.Dispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void ConfigureServices(IServiceCollection services) { }
        protected virtual void ConfigureCoreBuilder(ICoreBuilder coreBuilder, IConfigurationRoot configuration) { }
        protected virtual void ConfigureConfig(Config config) { }
    }
}