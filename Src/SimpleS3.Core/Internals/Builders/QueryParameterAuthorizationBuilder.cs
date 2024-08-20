using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Core.Internals.Builders;

internal sealed class QueryParameterAuthorizationBuilder(ISignatureBuilder builder, ILogger<QueryParameterAuthorizationBuilder> logger) : IAuthorizationBuilder
{
    public void BuildAuthorization(IRequest request)
    {
        Validator.RequireNotNull(request);

        logger.LogTrace("Building parameter based authorization");
        request.SetQueryParameter(AmzParameters.XAmzSignature, builder.CreateSignature(request, false).HexEncode());
    }
}