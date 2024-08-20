using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Core.Tests.Code.Handlers;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests;

/// <summary>Tests when the network does not work</summary>
public sealed class NetworkErrorTests(ITestOutputHelper helper) : OfflineTestBase(helper)
{
    private readonly BaseFailingHttpHandler _handler = new NetworkFailingHttpHandler(1);

    protected override void ConfigureCoreBuilder(ICoreBuilder coreBuilder, IConfigurationRoot configuration)
    {
        coreBuilder.UseHttpClientFactory()
                   .ConfigurePrimaryHttpMessageHandler(() => _handler)
                   .UseRetryAndTimeout(x =>
                   {
                       x.Retries = 3;
                       x.RetryMode = RetryMode.NoDelay;
                   });

        base.ConfigureCoreBuilder(coreBuilder, configuration);
    }

    [Fact]
    public async Task TestNonTransientNetworkError()
    {
        using MemoryStream ms = new MemoryStream(new byte[4096]);

        // One request should succeed
        PutObjectResponse response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestNonTransientNetworkError) + "-0", ms);

        Assert.True(response.IsSuccess);
        Assert.Equal(1, _handler.RequestCounter);

        // Second request should fail with a network error
        await Assert.ThrowsAsync<IOException>(async () => await ObjectClient.PutObjectAsync(BucketName, nameof(TestNonTransientNetworkError) + "-1", ms).ConfigureAwait(false));

        // Because network errors are transient, they should be retried
        Assert.Equal(5, _handler.RequestCounter);
    }

    protected override void Dispose(bool disposing)
    {
        _handler.Dispose();
        base.Dispose(disposing);
    }
}