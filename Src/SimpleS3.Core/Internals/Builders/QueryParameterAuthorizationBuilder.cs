using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Core.Internals.Builders;

internal sealed partial class QueryParameterAuthorizationBuilder(ISignatureBuilder builder, ILogger<QueryParameterAuthorizationBuilder> logger) : IAuthorizationBuilder
{
    public void BuildAuthorization(IRequest request)
    {
        Validator.RequireNotNull(request);

        BuildingParameterAuthorization();

        byte[]? signature = null;

        try
        {
            signature = builder.CreateSignature(request, false);
            request.SetQueryParameter(AmzParameters.XAmzSignature, signature.HexEncode());
        }
        finally
        {
            if (signature != null)
                Array.Clear(signature, 0, signature.Length);
        }
    }
}