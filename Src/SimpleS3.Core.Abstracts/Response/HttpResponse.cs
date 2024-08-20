namespace Genbox.SimpleS3.Core.Abstracts.Response;

public readonly struct HttpResponse(Stream? content, IDictionary<string, string> headers, int statusCode)
{
    public Stream? Content { get; } = content;
    public IDictionary<string, string> Headers { get; } = headers;
    public int StatusCode { get; } = statusCode;
}