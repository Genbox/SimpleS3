# SimpleS3.Extensions.Wasabi
This extension adds support for [Wasabi's S3 serivce](https://wasabi.com/s3-compatible-cloud-storage/).

To use it, add a reference to [Genbox.SimpleS3.Extensions.Wasabi](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.Wasabi)

### Using Microsoft.Extensions.DependencyInjection
If you are using [Microsoft's dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (recommended), then you can use it like this:

```csharp
ServiceCollection services = new ServiceCollection();
ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services);
IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();

coreBuilder.UseWasabi(config =>
{
    config.Region = WasabiRegion.EuCentral1;
    config.Credentials = new StringAccessKey("key id here", "access key here");
});

IServiceProvider serviceProvider = services.BuildServiceProvider();
IObjectClient objectClient = serviceProvider.GetRequiredService<IObjectClient>();
```

You can now use the `objectClient` to work with objects.