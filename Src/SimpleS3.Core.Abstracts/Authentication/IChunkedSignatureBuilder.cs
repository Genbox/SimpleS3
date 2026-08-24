using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Authentication;

/// <summary>Provides the <c>IChunkedSignatureBuilder</c> API.</summary>
public interface IChunkedSignatureBuilder
{
    /// <summary>Provides the <c>CreateChunkSignature</c> API.</summary>
    byte[] CreateChunkSignature(IRequest request, byte[] previousSignature, byte[] content, int offset, int length);
}