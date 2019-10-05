namespace Genbox.SimpleS3.Abstracts.Authentication
{
    public interface IAuthorizationBuilder
    {
        string BuildAuthorization(IRequest request);
    }
}