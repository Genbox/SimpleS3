namespace Genbox.SimpleS3.Core.Abstracts.Authentication
{
    public interface IAuthorizationBuilder
    {
        void BuildAuthorization(IRequest request);
    }
}