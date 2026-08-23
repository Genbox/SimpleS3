using System.Reflection;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Internals.Fluent;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class FluentUploadTests
{
    [Fact]
    public void CalculateContentMd5HashesEntireStreamAndRestoresPosition()
    {
        byte[] data = [1, 2, 3, 4];
        using MemoryStream content = new MemoryStream(data);
        content.Position = 2;
        Upload upload = new Upload(null!, null!, "bucket", "object");
        PutObjectRequest request = GetRequest(upload);
        request.Initialize("bucket", "object", content);

        upload.CalculateContentMd5();

        Assert.Equal(CryptoHelper.Md5Hash(data), request.ContentMd5);
        Assert.Equal(2, content.Position);
    }

    private static PutObjectRequest GetRequest(Upload upload)
    {
        FieldInfo field = typeof(Upload).GetField("_request", BindingFlags.Instance | BindingFlags.NonPublic)!;
        return (PutObjectRequest)field.GetValue(upload)!;
    }
}