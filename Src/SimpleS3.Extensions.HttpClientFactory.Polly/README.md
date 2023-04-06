# SimpleS3.Extensions.HttpClientFactory.Polly

This extension adds retry, timeout and error handling to
the [Genbox.SimpleS3.Extensions.HttpClientFactory](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.HttpClientFactory) extension using Polly.

To use it, install the package [Genbox.SimpleS3.Extensions.HttpClientFactory.Polly](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.HttpClientFactory.Polly)

### Using Microsoft.Extensions.DependencyInjection

If you are using [Microsoft's dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (recommended), then you simply add the extension like
this:

```csharp
ServiceCollection services = new ServiceCollection();
ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services);

IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();

//Use the default policy. Retry 3 times and 10 minute timeout
httpBuilder.UseDefaultHttpPolicy();

//Or specify your own retry policy with 5 retries
httpBuilder.UseRetryPolicy(5);

//Or specify your own timeout policy with 1 minute timeout
httpBuilder.UseTimeoutPolicy(TimeSpan.FromMinutes(1));
```