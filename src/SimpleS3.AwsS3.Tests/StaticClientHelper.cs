using Genbox.SimpleS3.AwsS3.Extensions;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.AwsS3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Genbox.SimpleS3.AwsS3.Tests
{
    public static class StaticClientHelper
    {
        public static (FakeNetworkDriver driver, S3Client client) CreateFakeClient()
        {
            FakeNetworkDriver fakeNetworkDriver = new FakeNetworkDriver();

            ServiceCollection services = new ServiceCollection();
            services.AddAwsS3();

            services.AddSingleton<IUrlBuilder, AwsUrlBuilder>();
            services.AddSingleton<IInputValidator, NullInputValidator>();

            services.Replace(ServiceDescriptor.Singleton<INetworkDriver>(fakeNetworkDriver));
            services.Replace(ServiceDescriptor.Singleton<ILoggerFactory, NullLoggerFactory>());
            services.Configure<Config>(x =>
            {
                x.Credentials = new StringAccessKey("ExampleKeyId00000000", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY");
                x.RegionCode = "us-east-1";
            });

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            IObjectClient objectClient = serviceProvider.GetRequiredService<IObjectClient>();
            IBucketClient bucketClient = serviceProvider.GetRequiredService<IBucketClient>();
            IMultipartClient multipartClient = serviceProvider.GetRequiredService<IMultipartClient>();
            IMultipartTransfer multipartTransfer = serviceProvider.GetRequiredService<IMultipartTransfer>();
            ITransfer transfer = serviceProvider.GetRequiredService<ITransfer>();

            S3Client client = new S3Client(objectClient, bucketClient, multipartClient, multipartTransfer, transfer);

            return (fakeNetworkDriver, client);
        }
    }
}