# SimpleS3.Extensions.HttpClientFactory

This extension provides more advanced HTTP capabilities than the SimpleS3.Extensions.HttpClient network driver. It is based on
the [HttpClientFactory](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests)
class provided in the [Microsoft.Extensions.Http](https://www.nuget.org/packages/Microsoft.Extensions.Http/) nuget package.
It is the default HTTP driver in SimpleS3, so you don't have to do anything if you are using the [Genbox.SimpleS3](https://www.nuget.org/packages/Genbox.SimpleS3) package.

However, if you want to build your own SimpleS3 setup, add a reference to [Genbox.SimpleS3.Core](https://www.nuget.org/packages/Genbox.SimpleS3.Core)
and [Genbox.SimpleS3.Extensions.HttpClientFactory](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.HttpClientFactory).

### Using Microsoft.Extensions.DependencyInjection

If you are using [Microsoft's dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (recommended), then you simply add the driver like
this:

```csharp
ServiceCollection services = new ServiceCollection();
ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services);
coreBuilder.UseHttpClientFactory();
```