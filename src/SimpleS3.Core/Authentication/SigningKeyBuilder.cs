using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Genbox.SimpleS3.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Authentication
{
    public class SigningKeyBuilder : ISigningKeyBuilder
    {
        private readonly IS3ConfigCredentialProvider _credentialProvider;
        private readonly ILogger<SigningKeyBuilder> _logger;
        private readonly IOptions<S3Config> _options;

        public SigningKeyBuilder(IOptions<S3Config> options, IS3ConfigCredentialProvider credentialProvider, ILogger<SigningKeyBuilder> logger)
        {
            _options = options;
            _credentialProvider = credentialProvider;
            _logger = logger;
        }

        public byte[] CreateSigningKey(DateTimeOffset dateTime, string service)
        {
            _logger.LogTrace("Creating key for {Service}", service);

            //https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-header-based-auth.html
            //Documentation says the key is valid for 7 days, but tests shows that is not true.
            string date = dateTime.ToString(DateTimeFormats.Iso8601Date, DateTimeFormatInfo.InvariantInfo);

            byte[] accessKey = _credentialProvider.GetKey();

            byte[] key = Encoding.UTF8.GetBytes(SigningConstants.Scheme).Concat(accessKey).ToArray();
            byte[] hashDate = CryptoHelper.HmacSign(Encoding.UTF8.GetBytes(date), key);
            byte[] hashRegion = CryptoHelper.HmacSign(Encoding.UTF8.GetBytes(ValueHelper.EnumToString(_options.Value.Region)), hashDate);
            byte[] hashService = CryptoHelper.HmacSign(Encoding.UTF8.GetBytes(service), hashRegion);
            byte[] signingKey = CryptoHelper.HmacSign(Encoding.UTF8.GetBytes("aws4_request"), hashService);

            //Security: clear key material
            Array.Clear(accessKey, 0, accessKey.Length);
            Array.Clear(key, 0, key.Length);

            _logger.LogDebug("Signing key created: {SigningKey}", signingKey);

            return signingKey;
        }
    }
}