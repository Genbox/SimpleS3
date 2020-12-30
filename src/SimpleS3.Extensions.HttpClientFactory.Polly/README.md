# SimpleS3.Extensions.HttpClientFactory.Polly
This extension adds retry, timeout and error handling to the [Genbox.SimpleS3.Extensions.HttpClientFactory](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.HttpClientFactory) extension using Polly.

### Using Microsoft.Extensions.DependencyInjection
If you are using [Microsoft's dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (recommended), then you simply add the extension like this

```csharp
ServiceCollection services = new ServiceCollection();
ICoreBuilder coreBuilder = services.AddSimpleS3Core(s3Config =>
{
    s3Config.Credentials = new StringAccessKey(keyId, accessKey);
    s3Config.Region = region;
});

IHttpClientBuilder? httpBuilder = coreBuilder.UseHttpClientFactory();

//Use the default policy. Retry 3 times and 10 minute timeout
httpBuilder.UseDefaultHttpPolicy();

//Or specify your own retry policy with 5 retries
httpBuilder.UseRetryPolicy(5);

//Or specify your own timeout policy with 1 minute timeout
httpBuilder.UseTimeoutPolicy(TimeSpan.FromMinutes(1));

IServiceProvider serviceProvider = services.BuildServiceProvider();
IObjectClient objectClient = serviceProvider.GetRequiredService<IObjectClient>();
```

You can now use the `objectClient` to work with objects on S3.