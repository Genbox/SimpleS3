using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Genbox.SimpleS3.Core.Tests.Code.Handlers
{
    internal abstract class BaseFailingHttpHandler : HttpMessageHandler
    {
        public int RequestCounter { get; set; }

        protected HttpContent GetEmptyXmlContent()
        {
            return new StringContent(string.Empty, Encoding.UTF8, MediaTypeNames.Application.Xml);
        }

        protected async Task ConsumeRequestAsync(HttpRequestMessage request)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Mimick regular HTTP handler, and use CopyToAsync() to let the HttpContent _write_ to our network stream
                // Using ReadAsStreamAsync() is entirely different, and will always buffer/reuse the retrieved stream (meant for _reading_ from the network)
                if (request.Content != null)
                    await request.Content.CopyToAsync(ms).ConfigureAwait(false);

                // Ensure we could read data
                Assert.True(ms.Length > 0);
            }
        }

        protected HttpResponseMessage CreateResponse(HttpRequestMessage request, HttpStatusCode statusCode)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = GetEmptyXmlContent(),
                RequestMessage = request
            };
        }
    }
}