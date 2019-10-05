namespace Genbox.SimpleS3.Abstracts.Authentication
{
    public interface ISignatureBuilder
    {
        byte[] CreateSignature(IRequest request);
    }
}