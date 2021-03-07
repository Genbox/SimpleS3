# SimpleS3.Extensions.AmazonS3
This extension adds support for [Amazon's S3 serivce](https://aws.amazon.com/s3/).

To use it, add a reference to [Genbox.SimpleS3.Extensions.AmazonS3](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.AmazonS3)

### Using Microsoft.Extensions.DependencyInjection
If you are using [Microsoft's dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (recommended), then you can use it like so:

```csharp
ServiceCollection services = new ServiceCollection();
ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services);
IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();

coreBuilder.UseAmazonS3(config =>
{
    config.Region = AmazonS3Region.EuWest1;
    config.Credentials = new StringAccessKey("key id here", "access key here");
});

IServiceProvider serviceProvider = services.BuildServiceProvider();
IObjectClient objectClient = serviceProvider.GetRequiredService<IObjectClient>();
```

You can now use the `objectClient` to work with objects on S3.