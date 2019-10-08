# SimpleS3

[![NuGet](https://img.shields.io/nuget/v/Genbox.SimpleS3.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Genbox.SimpleS3/)

### Description
A C# implementation of Amazon's S3 API with a focus on simplicity and performance. Download or upload an object with a single line of code.

### S3 features
These are the feature this library implements.
* Support creating, deleting and listing contents of buckets
* Support for download, upload, and deletion of objects
* Multipart upload and download support
* Streaming chunked encoding support

### API features
These are the features provided by this API implementation.
* Uses [HttpClientFactory](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests) for optimal performance and flexibility
* Dependency injection friendly
* Supports configuration binding via [Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.2)
* Extensive unit tests ensure correctness and stability
* Support for uploading/downloading multiparts in parallel
* Support for third-party servers like [Minio](https://min.io/) and [Wasabi](https://wasabi.com/)

### What is 'simple' about it?
The API is designed to be intuitive and easy to use while still providing expert users with advanced capabilities. The official AWS S3 SDK can be confusing to use as they are not using constructors of classes to indicate which parameters are required, and which are optional.
This API also supports uploading and downloading files as multiparts - exposed directly in the high-level client. It does not come at the expense of flexibility, as the lower-level APIs allow you to do everything yourself.

### What is 'secure' about it?
In today's cloud environments, it is more important than ever to handle encryption keys correctly. Keys are generated on machines that share memory with hundreds of other services, so it is important to limit the exposure of keys.
In SimpleS3, any derived keys are instantly zeroed from memory instead of waiting for the garbage collector. See [this](https://ianqvist.blogspot.com/2019/06/strings-from-security-perspective.html) for more details.

### What is 'performance' about it?
The library is built with performance in mind. The network layer is built to persist HTTP connections across API calls and all objects in the library are pooled to reduce garbage generation. The only garbage that is generated, is when you manually take control of requests and when .NET creates necessary garbage. The rest has been optimized away.

## Examples

#### Setup the config and client
```csharp
S3Client client = new S3Client("YourKeyId", "YourAccessKey", AwsRegion.EUWest1)
```

#### Or use Microsoft's Dependency Injection
```csharp
ServiceCollection services = new ServiceCollection();
services.AddSimpleS3Core(config => {
    config.Credentials = new SecretAccessKey("<YourKeyId>", "<YourAccessKey>");
    config.Region = AwsRegion.EUWest1;
}).UseHttpClientFactory();

ServiceProvider provider = services.BuildServiceProvider();
S3Client client = provider.GetRequiredService<IS3Client>();
```

#### Upload an object to a bucket
```csharp
await client.PutObjectStringAsync("MyBucket", "SimpleS3Object", "Hello World!");
```

#### Download an object
```csharp
string content = await client.GetObjectStringAsync("MyBucket", "SimpleS3Object");

if (content != null)
    Console.WriteLine(content); //Outputs: Hello World!
else
    Console.WriteLine("Something went wrong");
```

#### Delete an object
```csharp
await client.DeleteObjectAsync("MyBucket", "SimpleS3Object");
```

See the code in SimpleS3.Examples for more examples.
