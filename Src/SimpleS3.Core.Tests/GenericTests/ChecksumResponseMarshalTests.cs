using System.Diagnostics.CodeAnalysis;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class ChecksumResponseMarshalTests
{
    [Fact]
    public void CopyObjectReadsCompositeChecksumFromRootResult()
    {
        const string xml = """
                           <CopyObjectResult>
                             <ChecksumSHA512>AQID-2</ChecksumSHA512>
                             <ChecksumType>COMPOSITE</ChecksumType>
                             <ETag>"copy-etag"</ETag>
                           </CopyObjectResult>
                           """;

        using ContentStream content = CreateContent(xml);
        CopyObjectResponse response = new CopyObjectResponse();
        new CopyObjectResponseMarshal().MarshalResponse(new SimpleS3Config(), response, new Dictionary<string, string>(StringComparer.Ordinal), content);

        Assert.Equal(ChecksumAlgorithm.Sha512, response.ChecksumAlgorithm);
        Assert.Equal(ChecksumType.Composite, response.ChecksumType);
        Assert.Equal(new byte[] { 1, 2, 3 }, response.Checksum);
        Assert.Equal("\"copy-etag\"", response.ETag);
    }

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP001:Dispose created", Justification = "The returned ContentStream owns and disposes the memory stream.")]
    private static ContentStream CreateContent(string xml)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        return new ContentStream(stream, stream.Length);
    }
}