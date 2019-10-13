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

## Examples

#### Setup the config and client
```csharp
S3Client client = new S3Client("MyKeyId", "MyAccessKey", AwsRegion.EuWest1)
```

#### Or use Microsoft's Dependency Injection
```csharp
ServiceCollection services = new ServiceCollection();
services.AddSimpleS3(config => {
    config.Credentials = new SecretAccessKey("MyKeyId", "MyAccessKey");
    config.Region = AwsRegion.EuWest1;
});

ServiceProvider provider = services.BuildServiceProvider();
IS3Client client = provider.GetRequiredService<IS3Client>();
```

#### Upload an object to a bucket
```csharp
await client.PutObjectStringAsync("MyBucket", "MyObject", "Hello World!");
```

#### Download an object
```csharp
string content = await client.GetObjectStringAsync("MyBucket", "MyObject");

if (content != null)
    Console.WriteLine(content); //Outputs: Hello World!
else
    Console.WriteLine("Something went wrong");
```

#### Delete an object
```csharp
await client.DeleteObjectAsync("MyBucket", "MyObject");
```

### Fluid API
Tbe fluid API makes downloading/uploading objects easier by providing a convinient way of supplying information such as cache control, content disposition, encryption keys etc.
```csharp
//Upload string
Upload upload = client.Transfer
                 .UploadString("MyBucket", "MyObject", "Hello World!", Encoding.UTF8)
                 .WithAccessControl(ObjectCannedAcl.PublicReadWrite)
                 .WithCacheControl(CacheControlType.NoCache)
                 .WithEncryption();

PutObjectResponse resp = await upload.ExecuteAsync();
Console.WriteLine("Success? " + resp.IsSuccess);

//Download string
Download download = client.Transfer
                     .Download("MyBucket", "MyObject")
                     .WithRange(0, 5);                    

GetObjectResponse resp2 = await download.ExecuteAsync();
Console.WriteLine(await resp2.Content.AsStringAsync()); //outputs 'Hello'
```

See the code in SimpleS3.Examples for more examples.
