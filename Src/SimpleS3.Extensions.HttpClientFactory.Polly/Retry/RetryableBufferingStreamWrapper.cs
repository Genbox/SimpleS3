using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Retry;

internal class RetryableBufferingStreamWrapper : IRequestStreamWrapper
{
    public bool IsSupported(IRequest request) => true;

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP015:Member should not return created and cached instance")]
    public Stream Wrap(Stream input, IRequest request) => input.CanSeek ? input : new RetryableBufferingStream(input);
}