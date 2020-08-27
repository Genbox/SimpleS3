using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Retry
{
    internal class RetryableBufferingStreamWrapper : IRequestStreamWrapper
    {
        public bool IsSupported(IRequest request)
        {
            return true;
        }

        public Stream Wrap(Stream input, IRequest request)
        {
            if (input.CanSeek)
                return input;

            return new RetryableBufferingStream(input);
        }
    }
}