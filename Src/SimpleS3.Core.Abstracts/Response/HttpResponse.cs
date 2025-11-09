namespace Genbox.SimpleS3.Core.Abstracts.Response;

public readonly struct HttpResponse(ContentStream? content, IDictionary<string, string> headers, int statusCode)
{
    public ContentStream? Content { get; } = content;
    public IDictionary<string, string> Headers { get; } = headers;
    public int StatusCode { get; } = statusCode;
}