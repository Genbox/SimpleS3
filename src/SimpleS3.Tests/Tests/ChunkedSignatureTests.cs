using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Tests.Code.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Genbox.SimpleS3.Tests.Tests
{
    public class ChunkedSignatureTests
    {
        public ChunkedSignatureTests()
        {
            S3Config config = new S3Config(new StringAccessKey("AKIAIOSFODNN7EXAMPLE", "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY"), AwsRegion.UsEast1);

            _options = Options.Create(config);

            S3ConfigNullCredentialProvider nullCredsProvider = new S3ConfigNullCredentialProvider(_options);
            SigningKeyBuilder keyBuilder = new SigningKeyBuilder(_options, nullCredsProvider, new NullLogger<SigningKeyBuilder>());
            _scopeBuilder = new ScopeBuilder(_options);
            _sigBuilder = new SignatureBuilder(keyBuilder, _scopeBuilder, new NullLogger<SignatureBuilder>());
            _chunkedSigBuilder = new ChunkedSignatureBuilder(keyBuilder, _scopeBuilder, NullLogger<ChunkedSignatureBuilder>.Instance);
            _authHeaderBuilder = new AuthorizationHeaderBuilder(_options, _scopeBuilder, _sigBuilder, NullLogger<AuthorizationHeaderBuilder>.Instance);
        }

        private readonly SignatureBuilder _sigBuilder;
        private readonly ChunkedSignatureBuilder _chunkedSigBuilder;
        private readonly ScopeBuilder _scopeBuilder;
        private readonly DateTimeOffset _testDate = new DateTimeOffset(2013, 05, 24, 0, 0, 0, TimeSpan.Zero);
        private readonly AuthorizationHeaderBuilder _authHeaderBuilder;
        private readonly IOptions<S3Config> _options;

        [Fact]
        public void AmazonTestSuite()
        {
            //Test is based on the test example at the bottom here: https://docs.aws.amazon.com/AmazonS3/latest/API/sigv4-streaming.html

            string expectedSeedCr =
                "PUT\n" +
                "/examplebucket/chunkObject.txt\n" +
                "\n" +
                "content-encoding:aws-chunked\n" +
                "content-length:66824\n" +
                "host:s3.amazonaws.com\n" +
                "x-amz-content-sha256:STREAMING-AWS4-HMAC-SHA256-PAYLOAD\n" +
                "x-amz-date:20130524T000000Z\n" +
                "x-amz-decoded-content-length:66560\n" +
                "x-amz-storage-class:REDUCED_REDUNDANCY\n" +
                "\n" +
                "content-encoding;content-length;host;x-amz-content-sha256;x-amz-date;x-amz-decoded-content-length;x-amz-storage-class\n" +
                "STREAMING-AWS4-HMAC-SHA256-PAYLOAD";

            string expectedSeedSts =
                "AWS4-HMAC-SHA256\n" +
                "20130524T000000Z\n" +
                "20130524/us-east-1/s3/aws4_request\n" +
                "cee3fed04b70f867d036f722359b0b1f2f0e5dc0efadbc082b76c4c60e316455";

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(HttpHeaders.ContentEncoding, "aws-chunked");
            headers.Add(HttpHeaders.ContentLength, "66824");
            headers.Add(HttpHeaders.Host, "s3.amazonaws.com");
            headers.Add(AmzHeaders.XAmzContentSha256, "STREAMING-AWS4-HMAC-SHA256-PAYLOAD");
            headers.Add(AmzHeaders.XAmzDate, "20130524T000000Z");
            headers.Add(AmzHeaders.XAmzDecodedContentLength, "66560");
            headers.Add(AmzHeaders.XAmzStorageClass, "REDUCED_REDUNDANCY");

            IDictionary<string, string> query = new Dictionary<string, string>();

            //Override the header signing filter and just sign everything
            SigningConstants.ShouldSignHeader += s => true;

            string actualSeedCr = _sigBuilder.CreateCanonicalRequest(HttpMethod.PUT, "/examplebucket/chunkObject.txt", new ReadOnlyDictionary<string, string>(headers), new ReadOnlyDictionary<string, string>(query), "STREAMING-AWS4-HMAC-SHA256-PAYLOAD");
            Assert.Equal(expectedSeedCr, actualSeedCr);

            string scope = _scopeBuilder.CreateScope("s3", _testDate);
            string actualSts = _sigBuilder.CreateStringToSign(_testDate, scope, actualSeedCr);
            Assert.Equal(expectedSeedSts, actualSts);

            string expectedAuthHeader = "AWS4-HMAC-SHA256 Credential=AKIAIOSFODNN7EXAMPLE/20130524/us-east-1/s3/aws4_request,SignedHeaders=content-encoding;content-length;host;x-amz-content-sha256;x-amz-date;x-amz-decoded-content-length;x-amz-storage-class,Signature=4f232c4386841ef735655705268965c44a0e4690baa4adea153f7db9fa80a0a9";

            byte[] seedSignature = _sigBuilder.CreateSignature(_testDate, actualSts);
            string actualAuthHeader = _authHeaderBuilder.BuildHeader(_testDate, headers, seedSignature);
            Assert.Equal(expectedAuthHeader, actualAuthHeader);

            //Now we test the actual chunks

            byte[] file = new byte[65 * 1024]; //65 KB file
            file[0] = (byte)'a';

            int chunkSize = 64 * 1024; //64 KB

            List<List<byte>> chunks = file.Chunk(chunkSize).ToList();

            Assert.Equal(2, chunks.Count);

            byte[] first = chunks[0].ToArray();
            Assert.Equal(65536, first.Length);
            Assert.Equal(first[0], (byte)'a');

            string firstExpectedSts = "AWS4-HMAC-SHA256-PAYLOAD\n" +
                                      "20130524T000000Z\n" +
                                      "20130524/us-east-1/s3/aws4_request\n" +
                                      "4f232c4386841ef735655705268965c44a0e4690baa4adea153f7db9fa80a0a9\n" +
                                      "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855\n" +
                                      "d0fe33322334859574bfb4317205bf24106998f97d06a3a3c0e097d41db4c62f"; //"bf718b6f653bebc184e1479f1935b8da974d701b893afcf49e701f3e2f9f9c5a";

            string firstActualSts = _chunkedSigBuilder.CreateStringToSign(_testDate, scope, seedSignature, first, first.Length);
            Assert.Equal(firstExpectedSts, firstActualSts);

            string firstExpectedSig = "fe2879c55c032fbc6821af6fc8aac3c4b44939027344d8dceaa0aff08fe9ec43"; // "ad80c730a21e5b8d04586a2213dd63b9a0e99e0e2307b0ade35a65485a288648";
            string firstActualSig = _chunkedSigBuilder.CreateSignature(_testDate, firstActualSts).HexEncode();
            Assert.Equal(firstExpectedSig, firstActualSig);

            byte[] second = chunks[1].ToArray();
            Assert.Equal(1024, second.Length);

            string secondExpectedSts = "AWS4-HMAC-SHA256-PAYLOAD\n" +
                                       "20130524T000000Z\n" +
                                       "20130524/us-east-1/s3/aws4_request\n" +
                                       "fe2879c55c032fbc6821af6fc8aac3c4b44939027344d8dceaa0aff08fe9ec43\n" + //"ad80c730a21e5b8d04586a2213dd63b9a0e99e0e2307b0ade35a65485a288648\n" +
                                       "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855\n" +
                                       "5f70bf18a086007016e948b04aed3b82103a36bea41755b6cddfaf10ace3c6ef"; //"2edc986847e209b4016e141a6dc8716d3207350f416969382d431539bf292e4a";

            string secondActualSts = _chunkedSigBuilder.CreateStringToSign(_testDate, scope, firstActualSig.HexDecode(), second, second.Length);
            Assert.Equal(secondExpectedSts, secondActualSts);

            string secondExpectedSig = "970a9d34c5f01ef4f095b4516b9d8f34e8685df90024915ee8fd14278d361b00"; //"0055627c9e194cb4542bae2aa5492e3c1575bbb81b612b7d234b86a503ef5497";
            string secondActualSig = _chunkedSigBuilder.CreateSignature(_testDate, secondActualSts).HexEncode();
            Assert.Equal(secondExpectedSig, secondActualSig);

            byte[] third = Array.Empty<byte>();

            string thirdExpectedSts = "AWS4-HMAC-SHA256-PAYLOAD\n" +
                                      "20130524T000000Z\n" +
                                      "20130524/us-east-1/s3/aws4_request\n" +
                                      "970a9d34c5f01ef4f095b4516b9d8f34e8685df90024915ee8fd14278d361b00\n" + //"0055627c9e194cb4542bae2aa5492e3c1575bbb81b612b7d234b86a503ef5497\n" +
                                      "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855\n" +
                                      "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

            string thirdActualSts = _chunkedSigBuilder.CreateStringToSign(_testDate, scope, secondActualSig.HexDecode(), third, third.Length);
            Assert.Equal(thirdExpectedSts, thirdActualSts);

            string thirdExpectedSig = "433b9e5990bba9945ee26d418d377ae25cbf2a7db3e2d508a2b84e31e3c2d655"; //"b6c6ea8a5354eaf15b3cb7646744f4275b71ea724fed81ceb9323e279d449df9";
            string thirdActualSig = _chunkedSigBuilder.CreateSignature(_testDate, thirdActualSts).HexEncode();
            Assert.Equal(thirdExpectedSig, thirdActualSig);
        }

        [Fact]
        public void StreamTest()
        {
            UploadPartRequest req = new UploadPartRequest("examplebucket", "myresource", 1, "someid", new MemoryStream(Encoding.UTF8.GetBytes("Hello World")));
            req.Date = new DateTimeOffset(2019, 1, 1, 12, 0, 0, TimeSpan.Zero);
            req.AddHeader(AmzHeaders.XAmzContentSha256, "STREAMING-AWS4-HMAC-SHA256-PAYLOAD");

            byte[] seedSignature = _sigBuilder.CreateSignature(req);

            using (ChunkedStream stream = new ChunkedStream(_options, _chunkedSigBuilder, req, seedSignature, req.Content))
            using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
            {
                string expected =
                    "B;chunk-signature=11c8becc56fc860593b3c4fcf8c9450db27829b5d5d08136f6f0ddde7eccd03d\r\n" +
                    "Hello World\r\n" +
                    "0;chunk-signature=85ab1d2d5921c65c27ea3b3dedfcd68934f4cb52d8afe0f9315b9f0f6858f732\r\n\r\n";

                string actual = sr.ReadToEnd();
                Assert.Equal(expected, actual);
            }
        }
    }
}