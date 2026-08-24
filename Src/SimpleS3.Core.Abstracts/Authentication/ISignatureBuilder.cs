using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Authentication;

/// <summary>Provides the <c>ISignatureBuilder</c> API.</summary>
public interface ISignatureBuilder
{
    /// <summary>Provides the <c>CreateSignature</c> API.</summary>
    byte[] CreateSignature(IRequest request, bool enablePayloadSignature = true);
}