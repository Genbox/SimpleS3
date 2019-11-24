using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Genbox.SimpleS3.Abstracts.Authentication;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Tests.Code.Helpers;
using Genbox.SimpleS3.Tests.Code.Other;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Genbox.SimpleS3.Tests.Tests
{
    public class CanonicalRequestFileTests
    {
        private readonly ScopeBuilder _scopeBuilder;
        private readonly SignatureBuilder _sigBuilder;
        private readonly DateTimeOffset _testOffset = new DateTimeOffset(2015, 08, 30, 12, 36, 0, TimeSpan.Zero);

        public CanonicalRequestFileTests()
        {
            S3Config config = new S3Config(new StringAccessKey("AKIDEXAMPLE", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY"), AwsRegion.UsEast1);

            IOptions<S3Config> options = Options.Create(config);

            ISigningKeyBuilder keyBuilder = new SigningKeyBuilder(options, NullLogger<SigningKeyBuilder>.Instance);
            _scopeBuilder = new ScopeBuilder(options);
            _sigBuilder = new SignatureBuilder(keyBuilder, _scopeBuilder, NullLogger<SignatureBuilder>.Instance, options);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void TestFile(string content, string expectedCr, string expectedSts)
        {
            //Parse the request
            HttpHandler hh = HttpHelper.ParseHttpRequest(content);

            //Read the canonical request from the test suite
            string encodedUrl = UrlHelper.UrlPathEncode(hh.Target);

            string actualCr = _sigBuilder.CreateCanonicalRequest(Guid.Empty, encodedUrl, hh.Method, new ReadOnlyDictionary<string, string>(hh.Headers), new ReadOnlyDictionary<string, string>(hh.QueryParameters), CryptoHelper.Sha256Hash(hh.Body).HexEncode());

            Assert.Equal(expectedCr, actualCr);

            string scope = _scopeBuilder.CreateScope("service", _testOffset);

            string actualSts = _sigBuilder.CreateStringToSign(_testOffset, scope, actualCr);

            Assert.Equal(expectedSts, actualSts);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<object[]> Data()
        {
            //file-name.req: The web request to be signed.
            //file-name.creq: The resulting canonical request.
            //file-name.sts: The resulting string to sign.

            foreach ((string name, string content) in ResourceHelper.GetResources(@".*\.req$"))
            {
                string expectedCr = ResourceHelper.GetResource(Path.ChangeExtension(name, "creq"));
                string expectedSts = ResourceHelper.GetResource(Path.ChangeExtension(name, "sts"));

                yield return new object[] { content, expectedCr, expectedSts };
            }
        }
    }
}