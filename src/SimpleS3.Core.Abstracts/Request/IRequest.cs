using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

public interface IRequest
{
    Guid RequestId { get; set; }

    /// <summary>Date of the request</summary>
    DateTimeOffset Timestamp { get; set; }

    /// <summary>The method to use when performing the request</summary>
    HttpMethodType Method { get; }

    /// <summary>Headers to apply to the request</summary>
    IReadOnlyDictionary<string, string> Headers { get; }

    /// <summary>Query parameters to apply to the request</summary>
    IReadOnlyDictionary<string, string> QueryParameters { get; }

    /// <summary>Adds a query parameter</summary>
    /// <param name="key">Name of the parameter</param>
    /// <param name="value">Value of the parameter</param>
    void SetQueryParameter(string key, string value);

    /// <summary>Adds a header</summary>
    /// <param name="key">Name of the header</param>
    /// <param name="value">Value of the header</param>
    void SetHeader(string key, string value);
}