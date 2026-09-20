namespace Genbox.SimpleS3.Core.Abstracts.Response;

/// <summary>Provides the <c>HttpResponse</c> API.</summary>
public readonly struct HttpResponse(ContentStream? content, IDictionary<string, string> headers, int statusCode)
{
    /// <summary>Provides the <c>Content</c> API.</summary>
    public ContentStream? Content { get; } = content;

    /// <summary>Provides the <c>Headers</c> API.</summary>
    public IDictionary<string, string> Headers { get; } = headers;

    /// <summary>Provides the <c>StatusCode</c> API.</summary>
    public int StatusCode { get; } = statusCode;
}