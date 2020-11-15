using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genbox.SimpleS3.Core.Tests.Code.Helpers;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using HttpMethod = Genbox.SimpleS3.Core.Abstracts.Enums.HttpMethod;

namespace Genbox.SimpleS3.Core.Tests.Code.Handlers
{
    internal class HttpHandler : IHttpHeadersHandler, IHttpRequestLineHandler
    {
        public HttpHandler()
        {
            Headers = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Headers { get; }
        public IDictionary<string, string> QueryParameters { get; private set; }
        public HttpMethod Method { get; private set; }
        public string Target { get; private set; }
        public string Path { get; private set; }
        public string RawQuery { get; private set; }
        public byte[] Body { get; internal set; }

        public void OnHeader(Span<byte> name, Span<byte> value)
        {
            Headers.Add(Encoding.UTF8.GetString(name), Encoding.UTF8.GetString(value));
        }

        public void OnStartLine(Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod method, HttpVersion version, Span<byte> target, Span<byte> path, Span<byte> query, Span<byte> customMethod, bool pathEncoded)
        {
            Method = Enum.Parse<HttpMethod>(method.ToString().ToUpper());
            Target = Encoding.UTF8.GetString(target);

            //We need to remove parameters from the target
            int paramIndex = Target.IndexOf('?');

            if (paramIndex > 0)
                Target = Target.Substring(0, paramIndex);

            Path = Encoding.UTF8.GetString(path);
            RawQuery = Encoding.UTF8.GetString(query);
            QueryParameters = HttpHelper.ParseQueryString(RawQuery).ToDictionary(x => x.key, x => x.value, StringComparer.OrdinalIgnoreCase);
        }
    }
}