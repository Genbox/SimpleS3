using System;
using System.Collections.Generic;
using System.Linq;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Authentication
{
    public static class SigningConstants
    {
        internal const string Scheme = "AWS4";
        internal const string AlgorithmTag = Scheme + "-" + _algorithm;
        internal const string ChunkedAlgorithmTag = AlgorithmTag + "-PAYLOAD";
        private const string _amazonHeaderPrefix = "x-amz-";
        private const string _algorithm = "HMAC-SHA256";

        internal const char Slash = '/';
        internal const string SlashStr = "/";
        internal const char Newline = '\n';
        internal const char Colon = ':';
        internal const char SemiColon = ';';

        [PublicAPI]
        public static readonly ISet<string> HeaderWhitelist = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            //HttpHeaders.Host, //disabled for now because it is needed by presigned urls
            HttpHeaders.ContentType,
            HttpHeaders.ContentMd5
        };

        public static Func<string, bool> ShouldSignHeader { get; set; } = DefaultHeaderCheck;

        private static bool DefaultHeaderCheck(string header)
        {
            //Only amz headers: https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-header-based-auth.html
            return header.StartsWith(_amazonHeaderPrefix, StringComparison.Ordinal) || HeaderWhitelist.Contains(header);
        }

        public static IEnumerable<KeyValuePair<string, string>> FilterHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            foreach (KeyValuePair<string, string> item in headers.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
            {
                string loweredKey = item.Key.ToLowerInvariant();

                if (ShouldSignHeader(loweredKey))
                    yield return new KeyValuePair<string, string>(loweredKey, item.Value);
            }
        }
    }
}