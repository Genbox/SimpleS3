using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using Genbox.SimpleS3.Core.Common.Validation;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Tests.Helpers;

internal static class HttpHelper
{
    public static HttpHandler ParseHttpRequest(string request)
    {
        Validator.RequireNotNull(request, nameof(request));

        //Hack to change linux newlines into windows newlines
        request = request.Replace("\n", Environment.NewLine, false, CultureInfo.InvariantCulture);

        //Add a newline at the bottom too
        request += Environment.NewLine;

        HttpHandler handler = new HttpHandler();

        ReadOnlySequence<byte> buffer = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(request));
        HttpParser<HttpHandler> parser = new HttpParser<HttpHandler>();

        parser.ParseRequestLine(handler, buffer, out SequencePosition consumed, out _);
        buffer = buffer.Slice(consumed);

        parser.ParseHeaders(handler, buffer, out consumed, out _, out _);
        buffer = buffer.Slice(consumed);

        handler.Body = buffer.ToArray();

        return handler;
    }

    public static IEnumerable<(string key, string value)> ParseQueryString(string query)
    {
        if (string.IsNullOrEmpty(query))
            yield break;

        int queryLength = query.Length;
        int namePos = queryLength > 0 && query[0] == '?' ? 1 : 0;

        if (queryLength == namePos)
            yield break;

        while (namePos <= queryLength)
        {
            int valuePos = -1, valueEnd = -1;
            for (int q = namePos; q < queryLength; q++)
            {
                if (valuePos == -1 && query[q] == '=')
                    valuePos = q + 1;
                else if (query[q] == '&')
                {
                    valueEnd = q;
                    break;
                }
            }

            string name = HttpUtility.UrlDecode(query.Substring(namePos, valuePos - namePos - 1), Encoding.UTF8);

            if (valueEnd < 0)
                valueEnd = query.Length;

            namePos = valueEnd + 1;
            string value = HttpUtility.UrlDecode(query.Substring(valuePos, valueEnd - valuePos), Encoding.UTF8);
            yield return (name, value);
        }
    }
}