using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Internals.Fluent;
using Genbox.SimpleS3.Core.Internals.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class FluentUploadTests
{
    [Fact]
    public async Task CalculateContentMd5HashesRemainingStreamAndRestoresPosition()
    {
        byte[] data = [1, 2, 3, 4];
        using MemoryStream content = new MemoryStream(data);
        content.Position = 2;
        RecordingRequestHandler handler = new RecordingRequestHandler();
        ObjectOperations operations = new ObjectOperations(handler, [], []);
        Upload upload = new Upload(operations, null!, "bucket", "object");

        upload.CalculateContentMd5();
        Assert.Null(handler.Request);

        await upload.UploadAsync(content, TestContext.Current.CancellationToken);

        PutObjectRequest request = Assert.IsType<PutObjectRequest>(handler.Request);
        Assert.Equal(CryptoHelper.Md5Hash(data[2..]), request.ContentMd5);
        Assert.Equal(2, content.Position);
    }

    private sealed class RecordingRequestHandler : IRequestHandler
    {
        public IRequest? Request { get; private set; }

        public Task<TResp> SendRequestAsync<TReq, TResp>(TReq request, CancellationToken token = default) where TReq : IRequest where TResp : IResponse, new()
        {
            Request = request;
            return Task.FromResult(new TResp());
        }
    }
}