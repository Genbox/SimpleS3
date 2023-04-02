using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class RequestHelper
{
    public static void AppendPath(StringBuilder sb, SimpleS3Config config, IRequest request)
    {
        sb.Append('/');

        if (config.NamingMode == NamingMode.PathStyle && request is IHasBucketName bn)
            sb.Append(bn.BucketName).Append('/');

        if (request is IHasObjectKey ok)
            sb.Append(UrlHelper.UrlPathEncode(ok.ObjectKey));
    }

    public static void AppendQueryParameters(StringBuilder sb, IRequest request)
    {
        if (request.QueryParameters.Count > 0)
            sb.Append('?').Append(UrlHelper.CreateQueryString(request.QueryParameters));
    }
}