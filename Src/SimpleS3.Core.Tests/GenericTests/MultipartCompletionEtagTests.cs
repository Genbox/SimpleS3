using System.Xml.Linq;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class MultipartCompletionEtagTests
{
    [Fact]
    public void CompletionPreservesThePartEtagReturnedByUpload()
    {
        CompleteMultipartUploadRequest request = new CompleteMultipartUploadRequest("bucket", "object", "upload-id", new S3PartInfo("\"part-etag\"", 1));

        using Stream body = new CompleteMultipartUploadRequestMarshal().MarshalRequest(request, new SimpleS3Config());
        XDocument xml = XDocument.Load(body);

        Assert.Equal("\"part-etag\"", Assert.Single(xml.Descendants("ETag")).Value);
        Assert.Equal("\"part-etag\"", Assert.Single(request.UploadParts).ETag);
    }
}