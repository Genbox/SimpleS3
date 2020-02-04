using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common;
using Xunit;

namespace Genbox.SimpleS3.Core.Tests.Code.Other
{
    internal class TransientFailingHttpHandler : HttpMessageHandler
    {
        private readonly int _successRate;
        public int RequestCounter { get; set; }

        public TransientFailingHttpHandler(int successRate = 3)
        {
            Validator.RequireThat(successRate >= 1, nameof(successRate), "successRate must be greater than or equal 1");

            _successRate = successRate;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Mimick regular HTTP handler, and use CopyToAsync() to let the HttpContent _write_ to our network stream
                // Using ReadAsStreamAsync() is entirely different, and will always buffer/reuse the retrieved stream (meant for _reading_ from the network)
                await request.Content.CopyToAsync(ms).ConfigureAwait(false);

                // Ensure we could read data
                Assert.True(ms.Length > 0);
            }

            if (++RequestCounter % _successRate == 0)
            {
                // Success
                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

                responseMessage.Content = new StringContent(string.Empty, Encoding.UTF8, MediaTypeNames.Application.Xml);
                responseMessage.RequestMessage = request;

                return responseMessage;
            }

            // Transient error
            return new HttpResponseMessage(HttpStatusCode.RequestTimeout)
            {
                RequestMessage = request
            };
        }
    }
}