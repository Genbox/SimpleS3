# SimpleS3.Extensions.BackBlazeB2
This extension adds support for [BackBlaze's B2 serivce](https://www.backblaze.com/b2/cloud-storage.html).

To use it, add a reference to [Genbox.SimpleS3.Extensions.BackBlazeB2](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.BackBlazeB2)

### Using Microsoft.Extensions.DependencyInjection
If you are using [Microsoft's dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (recommended), then you can use B2 like so:

```csharp
ServiceCollection services = new ServiceCollection();
IS3ClientBuilder s3Builder = services.AddSimpleS3();

s3Builder.CoreBuilder.UseBackBlazeB2(config =>
{
    config.Region = B2Region.UsWest001;
    config.Credentials = new StringAccessKey("your application id here", "secret key here");
});

IServiceProvider serviceProvider = services.BuildServiceProvider();
IObjectClient objectClient = serviceProvider.GetRequiredService<IObjectClient>();
```

You can now use the `objectClient` to work with objects on B2.