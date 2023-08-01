# SimpleS3.Extensions.GenericS3

This extension supports any generic S3 endpoint. It does not provide any input validation as it is unknown what the generic endpoint would support.
If the endpoint you want to use is a close match to AmazonS3, then use SimpleS3.AmazonS3 instead.

To use it, add a reference to [Genbox.SimpleS3.Extensions.GenericS3](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.GenericS3)

### Using Microsoft.Extensions.DependencyInjection

If you are using [Microsoft's dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (recommended), then you can use it like so:

```csharp
ServiceCollection services = new ServiceCollection();
ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services);
IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();

coreBuilder.UseGenericS3(config =>
{
    config.Endpoint = new Uri("https://myendpoint.com");
    config.RegionCode = "us-east-1";
    config.Credentials = new StringAccessKey("key id here", "access key here");
});

IServiceProvider serviceProvider = services.BuildServiceProvider();
IObjectClient objectClient = serviceProvider.GetRequiredService<IObjectClient>();
```

You can now use the `objectClient` to work with objects.