namespace Genbox.SimpleS3.Core.Abstracts.Request;

public struct HttpResponse
{
    public HttpResponse(Stream? content, IDictionary<string, string> headers, int statusCode)
    {
        Content = content;
        Headers = headers;
        StatusCode = statusCode;
    }

    public Stream? Content { get; }
    public IDictionary<string, string> Headers { get; }
    public int StatusCode { get; }
}