using System.Xml.Linq;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class BucketLockConfigurationTests
{
    [Fact]
    public async Task DisablingObjectLockIsRejectedBeforeSending()
    {
        LockNetworkDriver driver = new LockNetworkDriver();
        await using ServiceProvider provider = CreateProvider(driver);
        IBucketClient client = provider.GetRequiredService<IBucketClient>();

        await Assert.ThrowsAsync<ArgumentException>(() => client.PutBucketLockConfigurationAsync("bucket", false, token: TestContext.Current.CancellationToken));
        Assert.Null(driver.Configuration);
    }

    [Fact]
    public async Task ClearingDefaultRetentionLeavesObjectLockEnabled()
    {
        LockNetworkDriver driver = new LockNetworkDriver();
        await using ServiceProvider provider = CreateProvider(driver);
        IBucketClient client = provider.GetRequiredService<IBucketClient>();

        PutBucketLockConfigurationResponse response = await client.ClearBucketDefaultRetentionAsync("bucket", request =>
        {
            request.LockMode = LockMode.Governance;
            request.LockRetainDays = 7;
            request.LockRetainYears = 1;
        }, TestContext.Current.CancellationToken);

        Assert.True(response.IsSuccess);
        Assert.NotNull(driver.Configuration);
        XNamespace ns = "http://s3.amazonaws.com/doc/2006-03-01/";
        Assert.Equal("Enabled", (string?)driver.Configuration.Element(ns + "ObjectLockEnabled"));
        Assert.Null(driver.Configuration.Element(ns + "Rule"));
    }

    private static ServiceProvider CreateProvider(LockNetworkDriver driver)
    {
        ServiceCollection services = new ServiceCollection();
        SimpleS3CoreServices.AddSimpleS3Core(services, config =>
        {
            config.Endpoint = "https://example.test";
            config.RegionCode = "us-east-1";
            config.NamingMode = NamingMode.PathStyle;
            config.Credentials = new StringAccessKey("key", "secret");
        });
        services.AddSingleton<INetworkDriver>(driver);
        return services.BuildServiceProvider();
    }

    private sealed class LockNetworkDriver : INetworkDriver
    {
        public XElement? Configuration { get; private set; }

        public async Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken cancellationToken = default) where T : IResponse
        {
            Assert.NotNull(requestStream);
            Configuration = await XElement.LoadAsync(requestStream, LoadOptions.None, cancellationToken);
            return new HttpResponse(null, new Dictionary<string, string>(StringComparer.Ordinal), 200);
        }
    }
}