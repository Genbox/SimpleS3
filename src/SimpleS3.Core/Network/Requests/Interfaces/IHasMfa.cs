using Genbox.SimpleS3.Core.Builders;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces
{
    public interface IHasMfa
    {
        /// <summary>If multi-factor approval is activated, you need to supply MFA information.</summary>
        MfaAuthenticationBuilder Mfa { get; }
    }
}