using Polly;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Internal;

internal class PollyHttpMessageHandler : DelegatingHandler
{
    private readonly Context _context;
    private readonly IAsyncPolicy<HttpResponseMessage> _policy;

    public PollyHttpMessageHandler(IAsyncPolicy<HttpResponseMessage> policy)
    {
        _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        _context = new Context();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _policy.ExecuteAsync((_, ct) => SendCoreAsync(request, ct), _context, cancellationToken).ConfigureAwait(false);
    }

    protected virtual Task<HttpResponseMessage> SendCoreAsync(HttpRequestMessage request, CancellationToken cancellationToken) => base.SendAsync(request, cancellationToken);
}