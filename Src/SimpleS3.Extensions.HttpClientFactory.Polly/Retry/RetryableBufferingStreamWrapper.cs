using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Retry;

internal sealed class RetryableBufferingStreamWrapper(IOptionsMonitor<PollyConfig> options, string optionsName) : IRequestStreamWrapper
{
    private readonly PollyConfig _config = options.Get(optionsName);

    public bool IsSupported(IRequest request) => true;

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP015:Member should not return created and cached instance", Justification = "Seekable input is returned unchanged; otherwise each call creates a new wrapper owned by the request pipeline.")]
    public Stream Wrap(Stream input, IRequest request) => input.CanSeek ? input : new RetryableBufferingStream(input, _config.MaxMemoryBufferSize);
}