namespace Genbox.SimpleS3.Abstracts.Authentication
{
    public interface IChunkedSignatureBuilder
    {
        byte[] CreateChunkSignature(IRequest request, byte[] previousSignature, byte[] content, int contentLength);
    }
}