using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Utils;

namespace Genbox.SimpleS3.Tests.Code.Other
{
    public class FakeHttpHandler : HttpMessageHandler
    {
        public string SendResource { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Validator.RequireNotNull(request, nameof(request));

            SendResource = request.RequestUri.AbsolutePath.TrimStart('/');
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(new byte[] {1, 2, 3});
            return Task.FromResult(response);
        }
    }
}