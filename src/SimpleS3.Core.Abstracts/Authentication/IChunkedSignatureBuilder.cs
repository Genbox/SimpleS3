namespace Genbox.SimpleS3.Core.Abstracts.Authentication
{
    public interface IChunkedSignatureBuilder
    {
        byte[] CreateChunkSignature(IRequest request, byte[] previousSignature, byte[] content, int offset, int length);
    }
}