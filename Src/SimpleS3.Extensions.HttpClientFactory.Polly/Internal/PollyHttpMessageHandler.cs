using Polly;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Internal;

internal sealed class PollyHttpMessageHandler(IAsyncPolicy<HttpResponseMessage> policy) : DelegatingHandler
{
    private readonly Context _context = new Context();
    private readonly IAsyncPolicy<HttpResponseMessage> _policy = policy ?? throw new ArgumentNullException(nameof(policy));

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _policy.ExecuteAsync((_, ct) => SendCoreAsync(request, ct), _context, cancellationToken).ConfigureAwait(false);
    }

    private Task<HttpResponseMessage> SendCoreAsync(HttpRequestMessage request, CancellationToken cancellationToken) => base.SendAsync(request, cancellationToken);
}