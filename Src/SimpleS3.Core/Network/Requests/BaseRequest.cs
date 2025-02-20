using System.Diagnostics;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests;

public abstract class BaseRequest(HttpMethodType method) : IRequest, IPooledObject, IHasExpectedBucketOwner
{
    private readonly Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _queryParameters = new Dictionary<string, string>(StringComparer.Ordinal);

    public virtual void Reset()
    {
        _headers.Clear();
        _queryParameters.Clear();
        ExpectedBucketOwner = null;
    }

    public Guid RequestId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public HttpMethodType Method { get; internal set; } = method;
    public IReadOnlyDictionary<string, string> Headers => _headers;
    public IReadOnlyDictionary<string, string> QueryParameters => _queryParameters;

    public void SetQueryParameter(string key, string value)
    {
        //Query parameters can contain different casing. 'partNumber' and 'uploadId' are two of them. They MUST be cased as such.
        _queryParameters[key] = value;
    }

    public void SetHeader(string key, string value)
    {
        Debug.Assert(!key.Any(char.IsUpper), "There was an uppercase character in the header: " + key);
        _headers[key] = value;
    }

    public string? ExpectedBucketOwner { get; set; }
}