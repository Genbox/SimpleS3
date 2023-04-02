using System.Net;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Core.Tests.Code.Handlers;

/// <summary>HTTP handler that fails with network errors after N requests</summary>
internal class NetworkFailingHttpHandler : BaseFailingHttpHandler
{
    private readonly int _afterNRequests;

    public NetworkFailingHttpHandler(int afterNRequests)
    {
        Validator.RequireThat(afterNRequests >= 1, "Value must be greater than or equal 1");

        _afterNRequests = afterNRequests;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (++RequestCounter > _afterNRequests)
        {
            // Throw IO exception
            throw new IOException("NIC is missing");
        }

        await ConsumeRequestAsync(request).ConfigureAwait(false);

        return CreateResponse(request, HttpStatusCode.OK);
    }
}