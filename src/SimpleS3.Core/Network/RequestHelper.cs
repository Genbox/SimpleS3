using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Internals.Helpers;

namespace Genbox.SimpleS3.Core.Network
{
    public static class RequestHelper
    {
        public static void AppendScheme(StringBuilder sb, Config config)
        {
            if (config.Endpoint == null)
                sb.Append(config.UseTLS ? "https" : "http");
            else
                sb.Append(config.Endpoint.Scheme);

            sb.Append("://");
        }

        public static void AppendQueryParameters<TReq>(StringBuilder sb, TReq request) where TReq : IRequest
        {
            if (request.QueryParameters.Count > 0)
                sb.Append('?').Append(UrlHelper.CreateQueryString(request.QueryParameters));
        }
    }
}