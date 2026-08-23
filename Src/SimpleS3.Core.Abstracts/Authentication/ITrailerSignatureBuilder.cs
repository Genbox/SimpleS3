using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Authentication;

public interface ITrailerSignatureBuilder
{
    byte[] CreateTrailerSignature(IRequest request, byte[] previousSignature);
}