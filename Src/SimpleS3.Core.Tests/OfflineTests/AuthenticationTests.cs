using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests;

public class AuthenticationTests
{
    [Fact]
    public void PreSignRequireCredentials()
    {
        ServiceCollection collection = new ServiceCollection();
        collection.Configure<SimpleS3Config>(config =>
        {
            config.Endpoint = "http://something.com";
            config.RegionCode = "us-east-1";
            config.NamingMode = NamingMode.PathStyle;
            config.Credentials = null;
        });

        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(collection);
        builder.UseHttpClientFactory();

        using ServiceProvider provider = collection.BuildServiceProvider();
        ISimpleClient client = provider.GetRequiredService<ISimpleClient>();

        Assert.Throws<InvalidOperationException>(() => client.SignRequest(new GetObjectRequest("test", "name"), TimeSpan.FromHours(1)));
    }
}