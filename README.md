# SimpleS3

[![License](https://img.shields.io/github/license/Genbox/SimpleS3)](https://github.com/Genbox/SimpleS3/blob/master/LICENSE.txt)

### Description

A C# implementation of Amazon's S3 API with a focus on simplicity, security and performance. Download or upload an object with a single line of code.

### S3 features

* Streaming chunked encoding support.
* Server-side encryption with customer keys.
* Support for path and virtual host style buckets.
* Support for pre-signed URLs.

### API features

Features provided by SimpleS3:

* Retry/timeout support via [Polly](https://github.com/App-vNext/Polly).
* Build requests through easy builder-based APIs. You never again have to remember the low-level details of a HTTP header.
* Use the ProfileManager to manage credentials securely with in-memory encryption.
* Full async and cancellation support.
* Client-side validation of requests makes it easier to know what is wrong before sending a request.
* Support uploading non-seekable streams or streams with no length.
* Supports automatic URL decoding of XML responses to support special non-XML compliant characters.
* Extensive unit tests ensure correctness and stability.
* Few allocations due to extensive use of object pools.
* API is fully annotated with [nullability](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references).
* Dependency injection friendly. By default uses [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/).
* Supports configuration binding via [Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.2).
* Support multiple S3 providers. See provider support further down for more info.
* Support for uploading/downloading multiparts with parallel requests.
* High-level API to help listing/deleting more than 1000 objects at the time. Uses [HTTP pipelining](https://en.wikipedia.org/wiki/HTTP_pipelining) to reduce latency.
* Automatic pooling of requests to reduce allocations and speed up requests.

### Usage

#### Download packages

- [![AmazonS3](https://img.shields.io/nuget/v/Genbox.SimpleS3.AmazonS3.svg?style=flat-square&label=Amazon%20S3)](https://www.nuget.org/packages/Genbox.SimpleS3.AmazonS3/)
- [![BackblazeB2](https://img.shields.io/nuget/v/Genbox.SimpleS3.BackBlazeB2.svg?style=flat-square&label=Backblaze%20B2)](https://www.nuget.org/packages/Genbox.SimpleS3.BackBlazeB2/)
- [![Google Cloud Storage](https://img.shields.io/nuget/v/Genbox.SimpleS3.GoogleCloudStorage.svg?style=flat-square&label=Google%20Cloud%20Storage)](https://www.nuget.org/packages/Genbox.SimpleS3.GoogleCloudStorage/)
- [![Wasabi](https://img.shields.io/nuget/v/Genbox.SimpleS3.Wasabi.svg?style=flat-square&label=Wasabi)](https://www.nuget.org/packages/Genbox.SimpleS3.Wasabi/)

See the [provider status page](https://github.com/Genbox/SimpleS3/wiki/Provider-status) for the list of operations that are currently supported. For the purpose of the examples
below, we are going to use the Amazon S3 provider.

#### Setup the config and client

```csharp
AmazonS3Client client = new AmazonS3Client("MyKeyId", "MyAccessKey", AmazonS3Region.EuWest1)
```

#### Or use Microsoft's Dependency Injection

```csharp
ServiceCollection services = new ServiceCollection();
services.AddAmazonS3(config => {
    config.Credentials = new StringAccessKey("MyKeyId", "MyAccessKey");
    config.Region = AmazonS3Region.EuWest1;
});

ServiceProvider provider = services.BuildServiceProvider();
AmazonS3Client client = provider.GetRequiredService<AmazonS3Client>();
```

#### Download/Upload using the Transfer API

The fluent Transfer API makes downloading/uploading objects easier by providing a convenient way of supplying information such as cache control, content-disposition, encryption
keys, etc.

```csharp
//Upload
IUpload upload = client.CreateUpload("MyBucket", "MyObject")
                        .WithAccessControl(ObjectCannedAcl.PublicReadWrite)
                        .WithCacheControl(CacheControlType.NoCache)
                        .WithEncryption();

PutObjectResponse putResp = await upload.UploadStringAsync("Hello World!");

//Download string
IDownload download = client.CreateDownload(bucketName, objectName)
                           .WithRange(0, 5);

GetObjectResponse getResp = await download.DownloadAsync();

Console.WriteLine(await dlResp.Content.AsStringAsync()); //Outputs "Hello"
```

### Extensions

SimpleS3 is extensible and has multiple different network drivers and extensions you can use. Click on the extension below for more information.

If you are already using the **SimpleS3.AmazonS3** package, you already have a dependency on most of the extensions below. These are only for advanced users that want to tweak
their setup to their own liking.

| Nuget                                                                                                                                                                                                                                 | Extension                                                                                                                 | Description                                                                                                                                                                                                                                     |
|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [![NuGet](https://img.shields.io/nuget/v/Genbox.SimpleS3.Extensions.HttpClient.svg?style=flat-square&label=HttpClient)](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.HttpClient/)                                        | [HttpClient](https://github.com/Genbox/SimpleS3/tree/master/Src/SimpleS3.Extensions.HttpClient)                           | A network driver based on HttpClient                                                                                                                                                                                                            |
| [![NuGet](https://img.shields.io/nuget/v/Genbox.SimpleS3.Extensions.HttpClientFactory.svg?style=flat-square&label=HttpClientFactory)](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.HttpClientFactory/)                   | [HttpClientFactory](https://github.com/Genbox/SimpleS3/tree/master/Src/SimpleS3.Extensions.HttpClientFactory)             | A network driver based on [HttpClientFactory](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests). It supports dynamic DNS changes. |
| [![NuGet](https://img.shields.io/nuget/v/Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.svg?style=flat-square&label=HttpClientFactory.Polly)](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.HttpClientFactory.Polly/) | [HttpClientFactory.Polly](https://github.com/Genbox/SimpleS3/tree/master/Src/SimpleS3.Extensions.HttpClientFactory.Polly) | Adds support for retry/timeout logic through [Polly](https://github.com/App-vNext/Polly)                                                                                                                                                        |
| [![NuGet](https://img.shields.io/nuget/v/Genbox.SimpleS3.Extensions.ProfileManager.svg?style=flat-square&label=ProfileManager)](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.ProfileManager/)                            | [ProfileManager](https://github.com/Genbox/SimpleS3/tree/master/Src/SimpleS3.Extensions.ProfileManager)                   | A profile manager that can safely persist your credentials to disk                                                                                                                                                                              |