using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>GetObjectResponse</c> API.</summary>
public class GetObjectResponse : HeadObjectResponse, IHasContent, IHasRequestCharged
{
    /// <summary>Provides the <c>Content</c> API.</summary>
    public ContentStream Content { get; internal set; } = ContentStream.Null;

    /// <summary>Provides the <c>Dispose</c> API.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }

    /// <summary>Provides the <c>Dispose</c> API.</summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            Content.Dispose();
    }
}