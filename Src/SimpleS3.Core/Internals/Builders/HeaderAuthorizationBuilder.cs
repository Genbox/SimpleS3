using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Pools;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Builders;

internal sealed class HeaderAuthorizationBuilder(IOptions<SimpleS3Config> options, IScopeBuilder scopeBuilder, ISignatureBuilder signatureBuilder, ILogger<HeaderAuthorizationBuilder> logger) : IAuthorizationBuilder
{
    private readonly SimpleS3Config _config = options.Value;

    public void BuildAuthorization(IRequest request)
    {
        Validator.RequireNotNull(request);

        string auth = BuildInternal(request.Timestamp, request.Headers, signatureBuilder.CreateSignature(request));

        request.SetHeader(HttpHeaders.Authorization, auth);
    }

    internal string BuildInternal(DateTimeOffset date, IReadOnlyDictionary<string, string> headers, byte[] signature)
    {
        logger.LogTrace("Building header based authorization");

        string scope = scopeBuilder.CreateScope("s3", date);

        StringBuilder header = StringBuilderPool.Shared.Rent(250);
        header.Append(SigningConstants.AlgorithmTag);
        header.AppendFormat(CultureInfo.InvariantCulture, " Credential={0}/{1},", _config.Credentials.KeyId, scope);
        header.AppendFormat(CultureInfo.InvariantCulture, "SignedHeaders={0},", string.Join(";", HeaderWhitelist.FilterHeaders(headers).Select(x => x.Key)));
        header.AppendFormat(CultureInfo.InvariantCulture, "Signature={0}", signature.HexEncode());

        string authHeader = StringBuilderPool.Shared.ReturnString(header);
        logger.LogDebug("AuthHeader: {AuthHeader}", authHeader);
        return authHeader;
    }
}