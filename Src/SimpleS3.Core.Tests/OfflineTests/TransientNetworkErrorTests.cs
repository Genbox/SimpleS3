﻿using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Core.Tests.Code.Handlers;
using Genbox.SimpleS3.Core.Tests.Code.Streams;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests;

public class TransientNetworkErrorTests(ITestOutputHelper helper) : OfflineTestBase(helper)
{
    private readonly BaseFailingHttpHandler _handler = new TransientFailingHttpHandler();

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
    public async Task TestTransientNetworkError()
    {
        using MemoryStream ms = new MemoryStream(new byte[4096]);

        PutObjectResponse response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestTransientNetworkError), ms);

        Assert.True(response.IsSuccess);
        Assert.True(_handler.RequestCounter >= 2);
    }

    [Fact]
    public async Task TestTransientNetworkError_NonSeekable()
    {
        await using NonSeekableStream ms = new NonSeekableStream(new byte[4096]);

        PutObjectResponse response = await ObjectClient.PutObjectAsync(BucketName, nameof(TestTransientNetworkError_NonSeekable), ms);

        Assert.True(response.IsSuccess);
        Assert.True(_handler.RequestCounter >= 2);
    }
}