using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Wrappers;

/// <summary>Provides the <c>IResponseWrapper</c> API.</summary>
public interface IResponseWrapper
{
    /// <summary>Provides the <c>IsSupported</c> API.</summary>
    bool IsSupported(IResponse response);

    /// <summary>Provides the <c>Wrap</c> API.</summary>
    ContentStream Wrap(ContentStream input, IResponse response);
}