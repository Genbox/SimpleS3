using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.TestBase.Helpers;
using Genbox.SimpleS3.Extensions.AwsS3.Extensions;
using Genbox.SimpleS3.Extensions.AwsS3.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Genbox.SimpleS3.Extensions.AwsS3.Tests
{
    public class CanonicalRequestFileTests
    {
        private readonly ScopeBuilder _scopeBuilder;
        private readonly SignatureBuilder _sigBuilder;
        private readonly DateTimeOffset _testOffset = new DateTimeOffset(2015, 08, 30, 12, 36, 0, TimeSpan.Zero);

        public CanonicalRequestFileTests()
        {
            ServiceCollection services = new ServiceCollection();
            SimpleS3CoreServices.AddSimpleS3Core(services).UseAwsS3(x =>
            {
                x.Credentials = new StringAccessKey("KeyIdExampleExampleE", "wJalrXUtnFEMI/K7MDENG+bPxRfiCYEXAMPLEKEY");
                x.Region = AwsRegion.UsEast1;
            });

            ServiceProvider? provider = services.BuildServiceProvider();

            _scopeBuilder = (ScopeBuilder)provider.GetRequiredService<IScopeBuilder>();
            _sigBuilder = (SignatureBuilder)provider.GetRequiredService<ISignatureBuilder>();
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

            Assembly assembly = typeof(CanonicalRequestFileTests).Assembly;

            foreach ((string name, string content) in ResourceHelper.GetResources(assembly, @".*\.req$"))
            {
                string expectedCr = ResourceHelper.GetResource(assembly, Path.ChangeExtension(name, "creq"));
                string expectedSts = ResourceHelper.GetResource(assembly, Path.ChangeExtension(name, "sts"));

                yield return new object[] { content, expectedCr, expectedSts };
            }
        }
    }
}