# SimpleS3.Extensions.BackBlazeB2
This extension adds support for [BackBlaze's B2 serivce](https://www.backblaze.com/b2/cloud-storage.html).
Note that this extension is only avaliable to [commercial tier sponsors](https://github.com/sponsors/Genbox).

To use it, add a reference to [Genbox.SimpleS3.Extensions.BackBlazeB2.Commercial](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.BackBlazeB2.Commercial)

### Using Microsoft.Extensions.DependencyInjection
If you are using [Microsoft's dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (recommended), then you can use B2 like so:

```csharp
ServiceCollection services = new ServiceCollection();
ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services);
IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();

coreBuilder.UseBackBlazeB2(config =>
{
    config.Region = B2Region.UsWest001;
    config.Credentials = new StringAccessKey("your application id here", "secret key here");
});

IServiceProvider serviceProvider = services.BuildServiceProvider();
IObjectClient objectClient = serviceProvider.GetRequiredService<IObjectClient>();
```

You can now use the `objectClient` to work with objects on B2.