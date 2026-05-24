using System.Net;
using System.Net.Mime;
using System.Text;

namespace Genbox.SimpleS3.Core.Tests.Code.Handlers;

internal abstract class BaseFailingHttpHandler : HttpMessageHandler
{
    public List<long?> ContentLengths { get; } = [];
    public List<byte[]> RequestBodies { get; } = [];

    public int RequestCounter { get; set; }

    protected HttpContent GetEmptyXmlContent() => new StringContent(string.Empty, Encoding.UTF8, MediaTypeNames.Application.Xml);

    protected async Task ConsumeRequestAsync(HttpRequestMessage request)
    {
        using MemoryStream ms = new MemoryStream();

        ContentLengths.Add(request.Content?.Headers.ContentLength);

        // Mimick regular HTTP handler, and use CopyToAsync() to let the HttpContent _write_ to our network stream
        // Using ReadAsStreamAsync() is entirely different, and will always buffer/reuse the retrieved stream (meant for _reading_ from the network)
        if (request.Content != null)
            await request.Content.CopyToAsync(ms).ConfigureAwait(false);

        RequestBodies.Add(ms.ToArray());

        // Ensure we could read data
        Assert.True(ms.Length > 0);
    }

    protected HttpResponseMessage CreateResponse(HttpRequestMessage request, HttpStatusCode statusCode) =>
        new HttpResponseMessage(statusCode)
        {
            Content = GetEmptyXmlContent(),
            RequestMessage = request
        };
}