namespace Genbox.SimpleS3.Core.Abstracts.Request;

public struct HttpResponse
{
    public Stream? Content { get; set; }
    public IDictionary<string, string> Headers { get; set; }
    public int StatusCode { get; set; }
}