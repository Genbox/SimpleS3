using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Microsoft.AspNetCore.DataProtection;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Internal.DataProtection;

internal class DataProtectionKeyProtector(IDataProtectionProvider provider) : IAccessKeyProtector
{
    private readonly IDataProtector _protector = provider.CreateProtector(nameof(DataProtectionKeyProtector));

    public byte[] ProtectKey(byte[] key) => _protector.Protect(key);

    public byte[] UnprotectKey(byte[] key) => _protector.Unprotect(key);
}