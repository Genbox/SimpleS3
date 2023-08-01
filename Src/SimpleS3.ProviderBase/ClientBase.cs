using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.ProviderBase;

public abstract class ClientBase : IDisposable
{
    private ServiceProvider? _serviceProvider;

    protected internal ClientBase(IInputValidator inputValidator, SimpleS3Config config, NetworkConfig? networkConfig = null)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton(inputValidator);
        services.AddSingleton(Options.Create(config));
        services.AddLogging();

        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(services);

        IHttpClientBuilder httpBuilder = builder.UseHttpClientFactory();

        if (networkConfig != null)
        {
            if (networkConfig.Proxy != null)
                httpBuilder.UseProxy(networkConfig.Proxy);

            httpBuilder.UseRetryAndTimeout(x =>
            {
                x.Retries = networkConfig.Retries;
                x.RetryMode = networkConfig.RetryMode;
                x.Timeout = networkConfig.Timeout;
                x.MaxRandomDelay = networkConfig.MaxRandomDelay;
            });
        }

        Client = Build(services);
    }

    protected internal ClientBase(IInputValidator inputValidator, SimpleS3Config config, INetworkDriver networkDriver)
    {
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton(inputValidator);
        services.AddLogging();

        SimpleS3CoreServices.AddSimpleS3Core(services);

        services.AddSingleton(networkDriver);
        services.AddSingleton(Options.Create(config));

        Client = Build(services);
    }

    protected internal ClientBase(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, IMultipartTransfer multipartTransfer, ITransfer transfer, ISignedObjectClient signedObjectClient)
    {
        Client = new SimpleClient(objectClient, bucketClient, multipartClient, multipartTransfer, transfer, signedObjectClient);
    }

    protected SimpleClient Client { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private SimpleClient Build(IServiceCollection services)
    {
        Validator.RequireThat(_serviceProvider == null, "You should only call Build once.");

        _serviceProvider = services.BuildServiceProvider();

        IObjectClient objectClient = _serviceProvider.GetRequiredService<IObjectClient>();
        IBucketClient bucketClient = _serviceProvider.GetRequiredService<IBucketClient>();
        IMultipartClient multipartClient = _serviceProvider.GetRequiredService<IMultipartClient>();
        IMultipartTransfer multipartTransfer = _serviceProvider.GetRequiredService<IMultipartTransfer>();
        ITransfer transfer = _serviceProvider.GetRequiredService<ITransfer>();
        ISignedObjectClient signedObjectClient = _serviceProvider.GetRequiredService<ISignedObjectClient>();

        return new SimpleClient(objectClient, bucketClient, multipartClient, multipartTransfer, transfer, signedObjectClient);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _serviceProvider?.Dispose();
    }
}