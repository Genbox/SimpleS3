using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Tests.Code.Other
{
    /// <summary>HTTP handler that fails with network errors after N requests</summary>
    internal class NetworkFailingHttpHandler : BaseFailingHttpHandler
    {
        private readonly int _afterNumRequests;

        public NetworkFailingHttpHandler(int afterNumRequests)
        {
            Validator.RequireThat(afterNumRequests >= 1, nameof(afterNumRequests), "afterNRequests must be greater than or equal 1");

            _afterNumRequests = afterNumRequests;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (++RequestCounter > _afterNumRequests)
                throw new IOException("NIC is missing");

            await ConsumeRequestAsync(request).ConfigureAwait(false);

            return CreateResponse(request, HttpStatusCode.OK);
        }
    }
}