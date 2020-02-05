using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Tests.Code.Other
{
    /// <summary>
    /// HTTP handler that fails with transient HTTP errors, except for each N requests
    /// </summary>
    internal class TransientFailingHttpHandler : BaseFailingHttpHandler
    {
        private readonly int _successRate;

        public TransientFailingHttpHandler(int successRate = 3)
        {
            Validator.RequireThat(successRate >= 1, nameof(successRate), "successRate must be greater than or equal 1");

            _successRate = successRate;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await ConsumeRequestAsync(request).ConfigureAwait(false);

            if (++RequestCounter % _successRate == 0)
            {
                // Success
                return CreateResponse(request, HttpStatusCode.OK);
            }

            // Transient error
            return CreateResponse(request, HttpStatusCode.RequestTimeout);
        }
    }
}