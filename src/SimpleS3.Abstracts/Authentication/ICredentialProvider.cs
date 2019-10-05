namespace Genbox.SimpleS3.Abstracts.Authentication
{
    public interface ICredentialProvider
    {
        byte[] GetKey();
    }
}