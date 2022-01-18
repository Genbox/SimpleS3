using System;
using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.TestBase
{
    public abstract class UnitTestBase : IDisposable
    {
        protected UnitTestBase(ITestOutputHelper outputHelper, string? profileName = null)
        {
            ProfileName = profileName;
            IConfigurationRoot configRoot = new ConfigurationBuilder()
                                            .AddJsonFile("Config.json", false)
                                            .Build();

            ServiceCollection collection = new ServiceCollection();
            collection.AddSingleton(configRoot);
            collection.Configure<SimpleS3Config>(configRoot);
            collection.Configure<SimpleS3Config>(ConfigureConfig);

            ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(collection);
            ConfigureCoreBuilder(coreBuilder, configRoot);

            collection.AddLogging(x =>
            {
                x.AddConfiguration(configRoot.GetSection("Logging"));
                x.AddXUnit(outputHelper);
            });

            //A small hack to remove all validators, as we test them separately
            collection.RemoveAll(typeof(IValidator<>));

            ConfigureServices(collection);

            Services = collection.BuildServiceProvider();
        }

        protected ServiceProvider Services { get; }
        protected string BucketName { get; set; }
        protected string? ProfileName { get; }
        protected IObjectOperations ObjectOperations => Services.GetRequiredService<IObjectOperations>();
        protected IObjectClient ObjectClient => Services.GetRequiredService<IObjectClient>();

        public virtual void Dispose()
        {
            Services?.Dispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void ConfigureServices(IServiceCollection services) { }
        protected virtual void ConfigureCoreBuilder(ICoreBuilder coreBuilder, IConfigurationRoot configuration) { }
        protected virtual void ConfigureConfig(SimpleS3Config config) { }
    }
}