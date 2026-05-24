using System.Text;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Errors;
using Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class ListResponseMarshalTests
{
    [Fact]
    public void ListObjectsDecodesCommonPrefixes()
    {
        const string xml = """
                           <ListBucketResult>
                             <EncodingType>url</EncodingType>
                             <CommonPrefixes><Prefix>foo%20bar/</Prefix></CommonPrefixes>
                           </ListBucketResult>
                           """;
        ListObjectsResponse response = new ListObjectsResponse();

        using ContentStream content = CreateContent(xml);
        new ListObjectsResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(), content);

        Assert.Equal("foo bar/", Assert.Single(response.CommonPrefixes));
    }

    [Fact]
    public void ListObjectVersionsDecodesCommonPrefixes()
    {
        const string xml = """
                           <ListVersionsResult>
                             <EncodingType>url</EncodingType>
                             <CommonPrefixes><Prefix>foo%20bar/</Prefix></CommonPrefixes>
                           </ListVersionsResult>
                           """;
        ListObjectVersionsResponse response = new ListObjectVersionsResponse();

        using ContentStream content = CreateContent(xml);
        new ListObjectVersionsResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(), content);

        Assert.Equal("foo bar/", Assert.Single(response.CommonPrefixes));
    }

    [Fact]
    public void ErrorXmlReaderRejectsDtds()
    {
        const string xml = """
                           <!DOCTYPE Error [ <!ENTITY xxe SYSTEM "file:///etc/passwd"> ]>
                           <Error><Code>InvalidArgument</Code><Message>&xxe;</Message></Error>
                           """;

        Assert.Throws<XmlException>(() => ErrorHandler.Create(XmlReader.Create(CreateContent(xml))));
    }

    private static SimpleS3Config CreateConfig() => new SimpleS3Config { AutoUrlDecodeResponses = true };

    private static ContentStream CreateContent(string xml)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        return new ContentStream(stream, stream.Length);
    }
}