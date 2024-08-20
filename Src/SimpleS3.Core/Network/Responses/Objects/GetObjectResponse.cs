using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public class GetObjectResponse : HeadObjectResponse, IHasContent, IHasRequestCharged
{
    public Stream Content { get; internal set; } = Stream.Null;

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