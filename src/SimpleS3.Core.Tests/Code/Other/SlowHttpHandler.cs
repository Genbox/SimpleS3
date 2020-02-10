using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Tests.Code.Other
{
    /// <summary>
    /// HTTP handler that delays all request, except each N requests
    /// </summary>
    internal class SlowHttpHandler : BaseFailingHttpHandler
    {
        private readonly int _successRate;
        private readonly TimeSpan _delay;

        public SlowHttpHandler(int successRate, TimeSpan delay)
        {
            Validator.RequireThat(successRate >= 1, nameof(successRate), "successRate must be greater than or equal 1");

            _successRate = successRate;
            _delay = delay;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await ConsumeRequestAsync(request).ConfigureAwait(false);

            // After N requests we should succeed
            if (++RequestCounter % _successRate == 0)
                return CreateResponse(request, HttpStatusCode.OK);

            // Delay for some time to let timeout occur
            await Task.Delay(_delay, cancellationToken).ConfigureAwait(false);

            return CreateResponse(request, HttpStatusCode.RequestTimeout);
        }
    }
}