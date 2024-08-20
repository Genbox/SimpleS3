using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Extensions.AmazonS3.Tests.Helpers;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Tests;

internal sealed class HttpHandler : IHttpHeadersHandler, IHttpRequestLineHandler
{
    public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public IDictionary<string, string> QueryParameters { get; private set; } = null!;
    public HttpMethodType Method { get; private set; } = HttpMethodType.Unknown;
    public string Target { get; private set; } = null!;
    public string RawQuery { get; private set; } = null!;
    public byte[] Body { get; internal set; } = null!;

    public void OnHeader(Span<byte> name, Span<byte> value)
    {
        Headers.Add(Encoding.UTF8.GetString(name), Encoding.UTF8.GetString(value));
    }

    public void OnStartLine(HttpMethod method, HttpVersion version, Span<byte> target, Span<byte> path, Span<byte> query, Span<byte> customMethod, bool pathEncoded)
    {
        Method = Enum.Parse<HttpMethodType>(method.ToString().ToUpperInvariant());
        Target = Encoding.UTF8.GetString(target);

        //We need to remove parameters from the target
        int paramIndex = Target.IndexOf('?', StringComparison.Ordinal);

        if (paramIndex > 0)
            Target = Target[..paramIndex];

        RawQuery = Encoding.UTF8.GetString(query);
        QueryParameters = HttpHelper.ParseQueryString(RawQuery).ToDictionary(x => x.key, x => x.value, StringComparer.OrdinalIgnoreCase);
    }
}