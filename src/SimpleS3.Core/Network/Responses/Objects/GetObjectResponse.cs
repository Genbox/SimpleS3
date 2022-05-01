using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public class GetObjectResponse : HeadObjectResponse, IHasContent, IHasRequestCharged
{
    public GetObjectResponse()
    {
        Content = Stream.Null;
    }

    public Stream Content { get; internal set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public bool RequestCharged { get; internal set; }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            Content.Dispose();
    }
}