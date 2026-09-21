using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Authentication;

/// <summary>Provides the <c>IAuthorizationBuilder</c> API.</summary>
public interface IAuthorizationBuilder
{
    /// <summary>Provides the <c>BuildAuthorization</c> API.</summary>
    void BuildAuthorization(IRequest request, string service);
}