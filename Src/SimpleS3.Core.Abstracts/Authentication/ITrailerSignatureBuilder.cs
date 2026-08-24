using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Authentication;

/// <summary>Provides the <c>ITrailerSignatureBuilder</c> API.</summary>
public interface ITrailerSignatureBuilder
{
    /// <summary>Provides the <c>CreateTrailerSignature</c> API.</summary>
    byte[] CreateTrailerSignature(IRequest request, byte[] previousSignature);
}