namespace Genbox.SimpleS3.Extensions.ProfileManager;

public class ProfileManagerOptions
{
    /// <summary>Controls whether the ProfileManager should clear the input key when creating profiles or not. Defaults to true.</summary>
    public bool ClearInputKey { get; set; } = true;
}