using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Authentication;

public interface ISignatureBuilder
{
    byte[] CreateSignature(IRequest request, bool enablePayloadSignature = true);
}