namespace Genbox.SimpleS3.Core.Abstracts.Authentication
{
    public interface IAuthorizationBuilder
    {
        string BuildAuthorization(IRequest request);
    }
}