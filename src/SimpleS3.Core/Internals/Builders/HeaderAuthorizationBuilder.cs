using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Common.Pools;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Builders;

internal class HeaderAuthorizationBuilder : IAuthorizationBuilder
{
    private readonly ILogger<HeaderAuthorizationBuilder> _logger;
    private readonly IOptions<SimpleS3Config> _options;
    private readonly IScopeBuilder _scopeBuilder;
    private readonly ISignatureBuilder _signatureBuilder;

    public HeaderAuthorizationBuilder(IOptions<SimpleS3Config> options, IScopeBuilder scopeBuilder, ISignatureBuilder signatureBuilder, ILogger<HeaderAuthorizationBuilder> logger)
    {
        _options = options;
        _scopeBuilder = scopeBuilder;
        _signatureBuilder = signatureBuilder;
        _logger = logger;
    }

    public void BuildAuthorization(IRequest request)
    {
        Validator.RequireNotNull(request, nameof(request));

        string auth = BuildInternal(request.Timestamp, request.Headers, _signatureBuilder.CreateSignature(request));

        request.SetHeader(HttpHeaders.Authorization, auth);
    }

    internal string BuildInternal(DateTimeOffset date, IReadOnlyDictionary<string, string> headers, byte[] signature)
    {
        _logger.LogTrace("Building header based authorization");

        string scope = _scopeBuilder.CreateScope("s3", date);

        StringBuilder header = StringBuilderPool.Shared.Rent(250);
        header.Append(SigningConstants.AlgorithmTag);
        header.AppendFormat(CultureInfo.InvariantCulture, " Credential={0}/{1},", _options.Value.Credentials.KeyId, scope);
        header.AppendFormat(CultureInfo.InvariantCulture, "SignedHeaders={0},", string.Join(";", HeaderWhitelist.FilterHeaders(headers).Select(x => x.Key)));
        header.AppendFormat(CultureInfo.InvariantCulture, "Signature={0}", signature.HexEncode());

        string authHeader = StringBuilderPool.Shared.ReturnString(header);
        _logger.LogDebug("AuthHeader: {AuthHeader}", authHeader);
        return authHeader;
    }
}