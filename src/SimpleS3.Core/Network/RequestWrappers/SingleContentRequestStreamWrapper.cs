using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Network.RequestWrappers
{
    [PublicAPI]
    public sealed class SingleContentRequestStreamWrapper : IRequestStreamWrapper
    {
        private readonly IOptions<S3Config> _config;
        private readonly ILogger<SingleContentRequestStreamWrapper> _logger;

        public SingleContentRequestStreamWrapper(IOptions<S3Config> config, ILogger<SingleContentRequestStreamWrapper> logger)
        {
            _config = config;
            _logger = logger;
        }

        public bool IsSupported(IRequest request)
        {
            return !_config.Value.EnableStreaming && _config.Value.EnablePayloadSigning;
        }

        public Stream Wrap(Stream input, IRequest request)
        {
            string contentHash = input == null ? "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855" : CryptoHelper.Sha256Hash(input, true).HexEncode();

            _logger.LogDebug("ContentSha256 is {ContentSha256}", contentHash);

            request.SetHeader(AmzHeaders.XAmzContentSha256, contentHash);

            return input;
        }
    }
}
