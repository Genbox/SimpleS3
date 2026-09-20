using System.Diagnostics.CodeAnalysis;
using Polly;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Internal;

internal sealed class PollyHttpMessageHandler(IAsyncPolicy<HttpResponseMessage> policy) : DelegatingHandler
{
    private readonly IAsyncPolicy<HttpResponseMessage> _policy = policy ?? throw new ArgumentNullException(nameof(policy));

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP001:Dispose created", Justification = "The request content owns its stream.")]
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => await _policy.ExecuteAsync((_, ct) => SendCoreAsync(request, ct), new Context(), cancellationToken);

    private Task<HttpResponseMessage> SendCoreAsync(HttpRequestMessage request, CancellationToken token) => base.SendAsync(request, token);
}