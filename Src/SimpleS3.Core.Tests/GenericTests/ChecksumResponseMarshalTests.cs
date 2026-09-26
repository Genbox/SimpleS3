using System.Diagnostics.CodeAnalysis;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart;
using Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
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

    [Fact]
    public void UploadPartCopyPrefersXmlChecksumOverHeader()
    {
        const string xml = """
                           <CopyPartResult><ChecksumXXHASH3>AQID-2</ChecksumXXHASH3><ETag>"part-etag"</ETag></CopyPartResult>
                           """;
        Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["x-amz-checksum-md5"] = "BAUG"
        };

        using ContentStream content = CreateContent(xml);
        UploadPartCopyResponse response = new UploadPartCopyResponse();
        new UploadPartCopyResponseMarshal().MarshalResponse(new SimpleS3Config(), response, headers, content);

        Assert.Equal(ChecksumAlgorithm.XxHash3, response.ChecksumAlgorithm);
        Assert.Equal(new byte[] { 1, 2, 3 }, response.Checksum);
        Assert.Equal("\"part-etag\"", response.ETag);
    }

    [Fact]
    public void CompleteMultipartUploadReadsExtendedCompositeChecksum()
    {
        const string xml = """
                           <CompleteMultipartUploadResult>
                             <ETag>"complete-etag"</ETag>
                             <ChecksumMD5>AQID-2</ChecksumMD5>
                             <ChecksumType>COMPOSITE</ChecksumType>
                           </CompleteMultipartUploadResult>
                           """;

        using ContentStream content = CreateContent(xml);
        CompleteMultipartUploadResponse response = new CompleteMultipartUploadResponse();
        new CompleteMultipartUploadResponseMarshal().MarshalResponse(new SimpleS3Config(), response, new Dictionary<string, string>(StringComparer.Ordinal), content);

        Assert.Equal(ChecksumAlgorithm.Md5, response.ChecksumAlgorithm);
        Assert.Equal(ChecksumType.Composite, response.ChecksumType);
        Assert.Equal(new byte[] { 1, 2, 3 }, response.Checksum);
        Assert.Equal("\"complete-etag\"", response.ETag);
    }

    [Fact]
    public void HeadObjectRecognizesExtendedCompositeChecksumHeader()
    {
        Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [AmzHeaders.XAmzChecksumSha512] = "AQID-2"
        };
        HeadObjectResponse response = new HeadObjectResponse();

        new HeadObjectResponseMarshal().MarshalResponse(new SimpleS3Config(), response, headers, ContentStream.Null);

        Assert.Equal(ChecksumAlgorithm.Sha512, response.ChecksumAlgorithm);
        Assert.Equal(new byte[] { 1, 2, 3 }, response.Checksum);
        Assert.Equal(ChecksumType.Composite, response.ChecksumType);
    }

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP001:Dispose created", Justification = "The returned ContentStream owns and disposes the memory stream.")]
    private static ContentStream CreateContent(string xml)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        return new ContentStream(stream, stream.Length);
    }
}