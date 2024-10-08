﻿using System.Diagnostics;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Core.Tests.Code.Handlers;
using Genbox.SimpleS3.Core.Tests.Code.Streams;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests;

/// <summary>Tests when the requests time out</summary>
public class TimeoutTests(ITestOutputHelper helper) : OfflineTestBase(helper)
{
    private readonly BaseFailingHttpHandler _handler = new SlowHttpHandler(2, TimeSpan.FromSeconds(5));

    protected override void ConfigureCoreBuilder(ICoreBuilder coreBuilder, IConfigurationRoot configuration)
    {
        coreBuilder.UseHttpClientFactory()
                   .ConfigurePrimaryHttpMessageHandler(() => _handler)
                   .UseRetryAndTimeout(3, TimeSpan.FromSeconds(3));

        base.ConfigureCoreBuilder(coreBuilder, configuration);
    }

    [Fact]
    public async Task TestClientCancellationToken()
    {
        using MemoryStream ms = new MemoryStream(new byte[4096]);
        using CancellationTokenSource tcs = new CancellationTokenSource();

        Task<PutObjectResponse> task = ObjectClient.PutObjectAsync(BucketName, nameof(TestClientCancellationToken), ms, token: tcs.Token);
        tcs.CancelAfter(500);

        Stopwatch sw = Stopwatch.StartNew();
        await Assert.ThrowsAsync<TaskCanceledException>(async () => await task.ConfigureAwait(false));
        sw.Stop();

        // We should have canceled within 750ms
        Assert.True(sw.ElapsedMilliseconds < 750);
    }

    [Fact]
    public async Task TestTimeoutError()
    {
        using MemoryStream ms = new MemoryStream(new byte[4096]);

        PutObjectResponse response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestTimeoutError), ms);

        // Request should succeed after N tries
        Assert.True(response.IsSuccess);
        Assert.Equal(2, _handler.RequestCounter);
    }

    [Fact]
    public async Task TestTimeoutError_NonSeekableStream()
    {
        await using NonSeekableStream ms = new NonSeekableStream(new byte[4096]);

        PutObjectResponse response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestTimeoutError_NonSeekableStream), ms);

        // Request should succeed after N tries
        Assert.True(response.IsSuccess);
        Assert.Equal(2, _handler.RequestCounter);
    }
}