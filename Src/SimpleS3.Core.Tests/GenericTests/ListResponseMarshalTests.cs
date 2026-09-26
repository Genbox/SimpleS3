using System.Diagnostics.CodeAnalysis;
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
        new ListObjectsResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(StringComparer.Ordinal), content);

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
        new ListObjectVersionsResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(StringComparer.Ordinal), content);

        Assert.Equal("foo bar/", Assert.Single(response.CommonPrefixes));
    }

    [Fact]
    public void ListObjectVersionsReadsSizesLargerThanInt32()
    {
        const string xml = """
                           <ListVersionsResult>
                             <Version>
                               <Key>large-object</Key><VersionId>v1</VersionId><IsLatest>true</IsLatest>
                               <LastModified>2026-09-26T12:00:00.000Z</LastModified><ETag>"etag"</ETag><Size>4294967296</Size>
                             </Version>
                           </ListVersionsResult>
                           """;

        using ContentStream content = CreateContent(xml);
        ListObjectVersionsResponse response = new ListObjectVersionsResponse();
        new ListObjectVersionsResponseMarshal().MarshalResponse(CreateConfig(), response, new Dictionary<string, string>(StringComparer.Ordinal), content);

        Assert.Equal(4294967296L, Assert.Single(response.Versions).Size);
    }

    [Fact]
    public void ListingsAcceptOwnersWithoutCanonicalIds()
    {
        const string objectsXml = """
                                  <ListBucketResult>
                                    <Contents><Key>one</Key><LastModified>2026-09-26T12:00:00Z</LastModified><Size>1</Size><Owner><DisplayName>alice</DisplayName></Owner></Contents>
                                    <Contents><Key>two</Key><LastModified>2026-09-26T12:00:00Z</LastModified><Size>2</Size></Contents>
                                  </ListBucketResult>
                                  """;
        const string versionsXml = """
                                   <ListVersionsResult>
                                     <Version><Key>one</Key><VersionId>v1</VersionId><IsLatest>true</IsLatest><LastModified>2026-09-26T12:00:00Z</LastModified><ETag>"etag"</ETag><Size>1</Size><Owner><DisplayName>alice</DisplayName></Owner></Version>
                                     <Version><Key>two</Key><VersionId>v2</VersionId><IsLatest>false</IsLatest><LastModified>2026-09-26T12:00:00Z</LastModified><ETag>"etag"</ETag><Size>2</Size></Version>
                                   </ListVersionsResult>
                                   """;

        using ContentStream objectsContent = CreateContent(objectsXml);
        ListObjectsResponse objects = new ListObjectsResponse();
        new ListObjectsResponseMarshal().MarshalResponse(CreateConfig(), objects, new Dictionary<string, string>(StringComparer.Ordinal), objectsContent);

        using ContentStream versionsContent = CreateContent(versionsXml);
        ListObjectVersionsResponse versions = new ListObjectVersionsResponse();
        new ListObjectVersionsResponseMarshal().MarshalResponse(CreateConfig(), versions, new Dictionary<string, string>(StringComparer.Ordinal), versionsContent);

        Assert.Equal("alice", objects.Objects[0].Owner?.Name);
        Assert.Equal(string.Empty, objects.Objects[0].Owner?.Id);
        Assert.Equal("two", objects.Objects[1].ObjectKey);
        Assert.Equal("alice", versions.Versions[0].Owner?.Name);
        Assert.Equal(string.Empty, versions.Versions[0].Owner?.Id);
        Assert.Equal("two", versions.Versions[1].ObjectKey);
    }

    [Fact]
    public void ErrorXmlReaderRejectsDtds()
    {
        const string xml = """
                           <!DOCTYPE Error [ <!ENTITY xxe SYSTEM "file:///etc/passwd"> ]>
                           <Error><Code>InvalidArgument</Code><Message>&xxe;</Message></Error>
                           """;

        using ContentStream content = CreateContent(xml);
        using XmlReader reader = XmlReader.Create(content);
        Assert.Throws<XmlException>(() => ErrorHandler.Create(reader));
    }

    private static SimpleS3Config CreateConfig() => new SimpleS3Config { AutoUrlDecodeResponses = true };

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP001:Dispose created", Justification = "The returned ContentStream owns and disposes the memory stream.")]
    private static ContentStream CreateContent(string xml)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        return new ContentStream(stream, stream.Length);
    }
}