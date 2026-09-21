namespace Genbox.SimpleS3.Core.Abstracts.Authentication;

/// <summary>Credentials issued by AWS STS or another AWS session credential provider.</summary>
public interface ISessionAccessKey : IAccessKey
{
    /// <summary>The session token that must be included in signed requests.</summary>
    string SessionToken { get; }
}