using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Builders;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Extensions.AmazonS3.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Tests;

public class ChunkedSignatureTests
{
    private readonly HeaderAuthorizationBuilder _authBuilder;
    private readonly ChunkedSignatureBuilder _chunkedSigBuilder;
    private readonly SimpleS3Config _config;
    private readonly ScopeBuilder _scopeBuilder;
    private readonly SignatureBuilder _sigBuilder;
    private readonly DateTimeOffset _testDate = new DateTimeOffset(2013, 05, 24, 0, 0, 0, TimeSpan.Zero);

    public ChunkedSignatureTests()
    {
        ServiceCollection services = new ServiceCollection();
        services.AddLogging();

        SimpleS3CoreServices.AddSimpleS3Core(services).UseAmazonS3(x =>
        {
            x.Credentials = new StringAccessKey("AKIAIOSFODNN7EXAMPLE", "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY");
            x.Region = AmazonS3Region.UsEast1;

            //The tests here have signatures built using path style
            x.NamingMode = NamingMode.PathStyle;
        });

        ServiceProvider provider = services.BuildServiceProvider();

        _scopeBuilder = (ScopeBuilder)provider.GetRequiredService<IScopeBuilder>();
        _sigBuilder = (SignatureBuilder)provider.GetRequiredService<ISignatureBuilder>();
        _chunkedSigBuilder = (ChunkedSignatureBuilder)provider.GetRequiredService<IChunkedSignatureBuilder>();
        _authBuilder = provider.GetRequiredService<HeaderAuthorizationBuilder>();
        _config = provider.GetRequiredService<IOptions<SimpleS3Config>>().Value;
    }

    [Fact]
    public void AmazonTestSuite()
    {
        //Test is based on the test example at the bottom here: https://docs.aws.amazon.com/AmazonS3/latest/API/sigv4-streaming.html

        const string expectedSeedCr = "PUT\n" +
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

        const string expectedSeedSts = "AWS4-HMAC-SHA256\n" +
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
        HeaderWhitelist.ShouldSignHeader += _ => true;

        string actualSeedCr = _sigBuilder.CreateCanonicalRequest(Guid.Empty, "/examplebucket/chunkObject.txt", HttpMethodType.PUT, new ReadOnlyDictionary<string, string>(headers), new ReadOnlyDictionary<string, string>(query), "STREAMING-AWS4-HMAC-SHA256-PAYLOAD");
        Assert.Equal(expectedSeedCr, actualSeedCr);

        string scope = _scopeBuilder.CreateScope("s3", _testDate);
        string actualSts = _sigBuilder.CreateStringToSign(_testDate, scope, actualSeedCr);
        Assert.Equal(expectedSeedSts, actualSts);

        const string expectedAuthHeader = "AWS4-HMAC-SHA256 Credential=AKIAIOSFODNN7EXAMPLE/20130524/us-east-1/s3/aws4_request,SignedHeaders=content-encoding;content-length;host;x-amz-content-sha256;x-amz-date;x-amz-decoded-content-length;x-amz-storage-class,Signature=4f232c4386841ef735655705268965c44a0e4690baa4adea153f7db9fa80a0a9";

        byte[] seedSignature = _sigBuilder.CreateSignature(_testDate, actualSts);
        string actualAuthHeader = _authBuilder.BuildInternal(_testDate, headers, seedSignature);
        Assert.Equal(expectedAuthHeader, actualAuthHeader);

        //Now we test the actual chunks

        byte[] file = new byte[65 * 1024]; //65 KB file
        file[0] = (byte)'a';

        const int chunkSize = 64 * 1024; //64 KB

        List<byte[]> chunks = file.Chunk(chunkSize).ToList();

        Assert.Equal(2, chunks.Count);

        byte[] first = chunks[0].ToArray();
        Assert.Equal(65536, first.Length);
        Assert.Equal((byte)'a', first[0]);

        const string firstExpectedSts = "AWS4-HMAC-SHA256-PAYLOAD\n" +
                                        "20130524T000000Z\n" +
                                        "20130524/us-east-1/s3/aws4_request\n" +
                                        "4f232c4386841ef735655705268965c44a0e4690baa4adea153f7db9fa80a0a9\n" +
                                        "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855\n" +
                                        "d0fe33322334859574bfb4317205bf24106998f97d06a3a3c0e097d41db4c62f"; //"bf718b6f653bebc184e1479f1935b8da974d701b893afcf49e701f3e2f9f9c5a";

        string firstActualSts = _chunkedSigBuilder.CreateStringToSign(_testDate, scope, seedSignature, first, 0, first.Length);
        Assert.Equal(firstExpectedSts, firstActualSts);

        const string firstExpectedSig = "fe2879c55c032fbc6821af6fc8aac3c4b44939027344d8dceaa0aff08fe9ec43"; // "ad80c730a21e5b8d04586a2213dd63b9a0e99e0e2307b0ade35a65485a288648";
        string firstActualSig = _chunkedSigBuilder.CreateSignature(_testDate, firstActualSts).HexEncode();
        Assert.Equal(firstExpectedSig, firstActualSig);

        byte[] second = chunks[1].ToArray();
        Assert.Equal(1024, second.Length);

        const string secondExpectedSts = "AWS4-HMAC-SHA256-PAYLOAD\n" +
                                         "20130524T000000Z\n" +
                                         "20130524/us-east-1/s3/aws4_request\n" +
                                         "fe2879c55c032fbc6821af6fc8aac3c4b44939027344d8dceaa0aff08fe9ec43\n" + //"ad80c730a21e5b8d04586a2213dd63b9a0e99e0e2307b0ade35a65485a288648\n" +
                                         "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855\n" +
                                         "5f70bf18a086007016e948b04aed3b82103a36bea41755b6cddfaf10ace3c6ef"; //"2edc986847e209b4016e141a6dc8716d3207350f416969382d431539bf292e4a";

        string secondActualSts = _chunkedSigBuilder.CreateStringToSign(_testDate, scope, firstActualSig.HexDecode(), second, 0, second.Length);
        Assert.Equal(secondExpectedSts, secondActualSts);

        const string secondExpectedSig = "970a9d34c5f01ef4f095b4516b9d8f34e8685df90024915ee8fd14278d361b00"; //"0055627c9e194cb4542bae2aa5492e3c1575bbb81b612b7d234b86a503ef5497";
        string secondActualSig = _chunkedSigBuilder.CreateSignature(_testDate, secondActualSts).HexEncode();
        Assert.Equal(secondExpectedSig, secondActualSig);

        byte[] third = [];

        const string thirdExpectedSts = "AWS4-HMAC-SHA256-PAYLOAD\n" +
                                        "20130524T000000Z\n" +
                                        "20130524/us-east-1/s3/aws4_request\n" +
                                        "970a9d34c5f01ef4f095b4516b9d8f34e8685df90024915ee8fd14278d361b00\n" + //"0055627c9e194cb4542bae2aa5492e3c1575bbb81b612b7d234b86a503ef5497\n" +
                                        "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855\n" +
                                        "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        string thirdActualSts = _chunkedSigBuilder.CreateStringToSign(_testDate, scope, secondActualSig.HexDecode(), third, 0, third.Length);
        Assert.Equal(thirdExpectedSts, thirdActualSts);

        const string thirdExpectedSig = "433b9e5990bba9945ee26d418d377ae25cbf2a7db3e2d508a2b84e31e3c2d655"; //"b6c6ea8a5354eaf15b3cb7646744f4275b71ea724fed81ceb9323e279d449df9";
        string thirdActualSig = _chunkedSigBuilder.CreateSignature(_testDate, thirdActualSts).HexEncode();
        Assert.Equal(thirdExpectedSig, thirdActualSig);
    }

    [Theory]
    [InlineData(0, "9bd7be022acc19c03c9d92444bf41f22dee2b55936b8e62d30ae8ebaa62dc2f6")]
    [InlineData(80 * 1024 - 1, "09e7b448c6d02db9448eabd564e9e7c01673581930c3a3bbced7b20adee96dec", "4839de3be25a94022f830ae1ae28987e026aa7ccc5467a76aa5e3272b2ff3028")]
    [InlineData(80 * 1024, "ffa0af214af9c70d10e78ac1bc15cc619bbaae6cf480608a0d15d3163228761a", "79a011a78294a3f8242b5e912f0240d1c38e8319061e5c0de5aad638d0fd8e2b")] // Default chunk size
    [InlineData(80 * 1024 + 1, "ffa0af214af9c70d10e78ac1bc15cc619bbaae6cf480608a0d15d3163228761a", "b3e03375420ec2f31f86567bdc715efb466c21a41ae217154cceaec265cd89ef", "c399aad5e87548b0798944ef24f4c4b3ee504193c04ff57d8a202a3ca3184778")]
    public void StreamTest(int dataSize, params string[] expectedSignatures)
    {
        byte[] originalData = new byte[dataSize];
        Array.Fill(originalData, (byte)'N');

        using MemoryStream memoryStream = new MemoryStream(originalData);

        UploadPartRequest req = new UploadPartRequest("examplebucket", "myresource", "someid", 1, memoryStream);
        req.Timestamp = new DateTimeOffset(2019, 1, 1, 12, 0, 0, TimeSpan.Zero);
        req.SetHeader(AmzHeaders.XAmzContentSha256, "STREAMING-AWS4-HMAC-SHA256-PAYLOAD");

        byte[] seedSignature = _sigBuilder.CreateSignature(req);

        using ChunkedStream stream = new ChunkedStream(_config, _chunkedSigBuilder, req, seedSignature, req.Content!);
        using StreamReader sr = new StreamReader(stream, Encoding.UTF8);
        StringBuilder sbExpected = new StringBuilder();

        int remaining = dataSize;
        for (int i = 0; i < expectedSignatures.Length; i++)
        {
            int size = Math.Min(_config.StreamingChunkSize, remaining);
            remaining -= size;

            sbExpected.Append(size.ToString("X", NumberFormatInfo.InvariantInfo));
            sbExpected.Append(";chunk-signature=");
            sbExpected.Append(expectedSignatures[i]);
            sbExpected.Append("\r\n");

            sbExpected.Append(new string('N', size));
            sbExpected.Append("\r\n");
        }

        string expected = sbExpected.ToString();

        string actual = sr.ReadToEnd();
        Assert.Equal(expected, actual);
    }
}