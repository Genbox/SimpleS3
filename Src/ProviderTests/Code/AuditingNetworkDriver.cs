using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.ProviderTests.Code;

internal sealed class AuditingNetworkDriver(INetworkDriver inner, string directory) : INetworkDriver
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = true };
    private long _sequence;

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP001:Dispose created", Justification = "The returned ContentStream owns the response audit stream.")]
    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP004:Don't ignore created IDisposable", Justification = "Response content ownership is transferred to the caller.")]
    public async Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken token = default) where T : IResponse
    {
        string exchange = Path.Combine(directory, $"{Interlocked.Increment(ref _sequence):D6}-{typeof(T).Name}");
        Directory.CreateDirectory(exchange);
        await WriteMetadataAsync(Path.Combine(exchange, "request.json"), new { Timestamp = DateTimeOffset.UtcNow, Method = request.Method.ToString(), Url = url, request.Headers }, token);

        await using AuditingReadStream? upload = requestStream == null ? null : new AuditingReadStream(requestStream, Path.Combine(exchange, "request.body"));
        HttpResponse response = default;

        try
        {
            response = await inner.SendRequestAsync<T>(request, url, upload, token);
            await WriteMetadataAsync(Path.Combine(exchange, "response.json"), new { Timestamp = DateTimeOffset.UtcNow, response.StatusCode, response.Headers }, token);
            if (response.Content == null)
                return response;

            AuditingReadStream download = new AuditingReadStream(response.Content, Path.Combine(exchange, "response.body"));
            return new HttpResponse(new ContentStream(download, response.Content.HasLength ? response.Content.Length : null), response.Headers, response.StatusCode);
        }
        catch (Exception ex)
        {
            if (response.Content != null)
                await response.Content.DisposeAsync();
            await File.WriteAllTextAsync(Path.Combine(exchange, "error.txt"), ex.ToString(), CancellationToken.None);
            throw;
        }
    }

    private static Task WriteMetadataAsync(string path, object value, CancellationToken token) => File.WriteAllTextAsync(path, JsonSerializer.Serialize(value, JsonOptions), token);
}