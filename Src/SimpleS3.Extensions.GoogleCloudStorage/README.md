# SimpleS3.Extensions.GoogleCloudStorage

This extension adds support for [Google's Cloud Storage (S3 compatible) serivce](https://cloud.google.com/storage/docs/migrating).

To use it, add a reference to [Genbox.SimpleS3.Extensions.GoogleCloudStorage](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.GoogleCloudStorage)

### Using Microsoft.Extensions.DependencyInjection

If you are using [Microsoft's dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (recommended), then you can use it like this:

```csharp
ServiceCollection services = new ServiceCollection();
ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services);
IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();

coreBuilder.UseGoogleCloudStorage(config =>
{
    config.Region = GoogleCloudStorageRegion.EuropeWest3;
    config.Credentials = new StringAccessKey("your key id here", "secret key here");
});

IServiceProvider serviceProvider = services.BuildServiceProvider();
IObjectClient objectClient = serviceProvider.GetRequiredService<IObjectClient>();
```

You can now use the `objectClient` to work with objects.