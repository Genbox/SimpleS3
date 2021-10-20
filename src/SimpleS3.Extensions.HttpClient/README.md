# SimpleS3.Extensions.HttpClient
This extension is a network driver that provides basic HTTP communication through the .NET System.Net.Http.HttpClient class. A single HttpClient instance is used for all connections. It supports web proxies.

To use it, add a reference to [Genbox.SimpleS3.Core](https://www.nuget.org/packages/Genbox.SimpleS3.Core) and [Genbox.SimpleS3.Extensions.HttpClient](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.HttpClient).

### Using Microsoft.Extensions.DependencyInjection
If you are using [Microsoft's dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (recommended), then you simply add the driver like this:

```csharp
ServiceCollection services = new ServiceCollection();
ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services);
coreBuilder.UseHttpClient();
```