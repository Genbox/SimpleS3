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

internal sealed partial class HeaderAuthorizationBuilder(IOptions<SimpleS3Config> options, IScopeBuilder scopeBuilder, ISignatureBuilder signatureBuilder, ILogger<HeaderAuthorizationBuilder> logger) : IAuthorizationBuilder
{
    private readonly SimpleS3Config _config = options.Value;

    public void BuildAuthorization(IRequest request, string service)
    {
        Validator.RequireNotNull(request);

        byte[]? signature = null;

        try
        {
            signature = signatureBuilder.CreateSignature(request, service);
            string auth = BuildInternal(request.Timestamp, request.Headers, signature, service);
            request.SetHeader(HttpHeaders.Authorization, auth);
        }
        finally
        {
            if (signature != null)
                Array.Clear(signature, 0, signature.Length);
        }
    }

    internal string BuildInternal(DateTimeOffset date, IReadOnlyDictionary<string, string> headers, byte[] signature, string service)
    {
        Validator.RequireNotNull(_config.Credentials, "If we get to this point, we expect to have valid credentials");
        BuildingHeaderAuthorization();

        string scope = scopeBuilder.CreateScope(service, date);

        StringBuilder header = StringBuilderPool.Shared.Rent(250);
        header.Append(SigningConstants.AlgorithmTag);
        header.AppendFormat(CultureInfo.InvariantCulture, " Credential={0}/{1},", _config.Credentials.KeyId, scope);
        header.AppendFormat(CultureInfo.InvariantCulture, "SignedHeaders={0},", string.Join(";", HeaderWhitelist.FilterHeaders(headers).Select(x => x.Key)));
        header.AppendFormat(CultureInfo.InvariantCulture, "Signature={0}", signature.HexEncode());

        string authHeader = StringBuilderPool.Shared.ReturnString(header, true);
        if (_config.LogSensitiveMaterial)
            LogAuthorizationHeader(authHeader);
        else
            AuthorizationHeaderCreated();

        return authHeader;
    }
}