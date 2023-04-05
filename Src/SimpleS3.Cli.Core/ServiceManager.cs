using Genbox.SimpleS3.Cli.Core.Abstracts;
using Genbox.SimpleS3.Cli.Core.Managers;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.AmazonS3.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Cli.Core;

public class ServiceManager
{
    private static ServiceManager? _serviceManager;
    private readonly ServiceProvider _provider;
    private BucketManager? _bucketManager;
    private IProfileSetup? _consoleSetup;
    private ObjectManager? _objectManager;
    private IProfileManager? _profileManager;

    private ServiceManager(string? profileName, string? endpoint, string? proxyUrl)
    {
        ServiceCollection services = new ServiceCollection();
        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(services);

        builder.UseProfileManager();

        IHttpClientBuilder httpBuilder = builder.UseHttpClientFactory()
                                                .UseRetryAndTimeout();

        if (proxyUrl != null)
            httpBuilder.UseProxy(proxyUrl);

        if (profileName != null)
        {
            services.Configure<SimpleS3Config>((x, y) =>
            {
                IProfileManager profileManager = y.GetRequiredService<IProfileManager>();
                x.UseProfile(profileManager, profileName);

                if (endpoint != null)
                    x.Endpoint = new Uri(endpoint);
            });
        }

        builder.UseAmazonS3();

        //services.AddSingleton<IInputValidator, NullValidator>();
        //services.AddSingleton<IRegionData, NullRegionData>();
        //services.AddSingleton<IUrlBuilder, NullUrlBuilder>();

        services.AddSingleton<BucketManager>();
        services.AddSingleton<ObjectManager>();
        services.AddSingleton<IFileProvider, DefaultFileProvider>();

        _provider = services.BuildServiceProvider();
    }

    public BucketManager BucketManager => _bucketManager ??= _provider.GetRequiredService<BucketManager>();
    public ObjectManager ObjectManager => _objectManager ??= _provider.GetRequiredService<ObjectManager>();
    public IProfileManager ProfileManager => _profileManager ??= _provider.GetRequiredService<IProfileManager>();
    public IProfileSetup ConsoleSetup => _consoleSetup ??= _provider.GetRequiredService<IProfileSetup>();

    public static ServiceManager GetInstance(string? profileName, string? endpoint, string? proxyUrl) => _serviceManager ??= new ServiceManager(profileName, endpoint, proxyUrl);
}