using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Internals.Pools;

namespace Genbox.SimpleS3.Core.Network.Requests;

public abstract class BaseRequest : IRequest, IPooledObject
{
    private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
    private readonly Dictionary<string, string> _queryParameters = new Dictionary<string, string>();

    protected BaseRequest(HttpMethodType method)
    {
        Method = method;
    }

    public virtual void Reset()
    {
        _headers.Clear();
        _queryParameters.Clear();
    }

    public Guid RequestId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public HttpMethodType Method { get; internal set; }
    public IReadOnlyDictionary<string, string> Headers => _headers;
    public IReadOnlyDictionary<string, string> QueryParameters => _queryParameters;

    public void SetQueryParameter(string key, string value)
    {
        _queryParameters[key] = value;
    }

    public void SetHeader(string key, string value)
    {
        _headers[key.ToLowerInvariant()] = value!;
    }
}