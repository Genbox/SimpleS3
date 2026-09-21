# SimpleS3.Extensions.HttpClientFactory.Polly

This extension adds retry, timeout and error handling to the [Genbox.SimpleS3.Extensions.HttpClientFactory](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.HttpClientFactory) extension using Polly.

To use it, install the package [Genbox.SimpleS3.Extensions.HttpClientFactory.Polly](https://www.nuget.org/packages/Genbox.SimpleS3.Extensions.HttpClientFactory.Polly)

### Using Microsoft.Extensions.DependencyInjection

If you are using [Microsoft's dependency injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (recommended), then you simply add the extension like this:

```csharp
ServiceCollection services = new ServiceCollection();
ICoreBuilder coreBuilder = SimpleS3CoreServices.AddSimpleS3Core(services);

IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();

// Three retries, with a ten-minute timeout for each HTTP attempt.
httpBuilder.UseRetryAndTimeout();

// Alternatively, choose both settings explicitly.
httpBuilder.UseRetryAndTimeout(5, TimeSpan.FromMinutes(1));

// Or configure the named policy in a callback.
httpBuilder.UseRetryAndTimeout(options =>
{
    options.Retries = 0;
    options.Timeout = TimeSpan.FromMinutes(1);
});
```

The registration examples are alternatives; choose one. Import `Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions` for these extension methods.

### What the policy covers

The default policy retries transport `IOException` and `HttpRequestException`, Polly timeouts, HTTP 408, and HTTP 5xx responses. Retryable responses are disposed before the delay, and seekable request content is rewound to its original position before replay. `Retries` counts additional attempts after the first send.

POST requests and requests containing `x-amz-request-token` use the timeout-only policy. They are not automatically replayed. Setting `Retries = 0` also disables retries for other requests while retaining the timeout.

`Timeout` applies separately to each HTTP send attempt until response headers arrive. The transport uses `ResponseHeadersRead`: later response-body reads, synchronous XML parsing, and embedded S3 errors in an HTTP-success response are outside this policy. Retry delays are also outside each attempt's timeout. A ten-minute timeout therefore does not limit a whole download or a sequence of retries to ten minutes.

When consuming a response body, pass the same cancellation token to the operation and to the body-copy or read call. For example, with an existing client and destination stream:

```csharp
using CancellationTokenSource cancellation = new CancellationTokenSource(TimeSpan.FromMinutes(1));
using var response = await client.GetObjectAsync(bucketName, objectKey, token: cancellation.Token);
if (!response.IsSuccess)
    throw new InvalidOperationException("The download request failed.");
await response.Content.CopyToAsync(destination, 81920, cancellation.Token);
```

Cancellation is cooperative; it does not interrupt synchronous XML parsing. The policy does not recover a partially read response body. Applications that need resumable downloads or an overall deadline must handle those requirements at the operation level, including object revision consistency and partial destination data.

### Forward-only uploads

The retry wrapper fully buffers a non-seekable source before returning its first buffered read or length. Data above `MaxMemoryBufferSize` spills to a temporary file. This makes the body replayable, but introduces startup latency and temporary storage proportional to the body size. Accessing the buffering stream's `Length` can also read the source synchronously.

The default threshold is 8 MiB; it is a buffering threshold, not an upload-size limit or a cap on total process memory. Account for the full buffering step when sizing temporary storage and measuring producer-to-network latency. The first network send need not begin as soon as the producer supplies its first bytes.