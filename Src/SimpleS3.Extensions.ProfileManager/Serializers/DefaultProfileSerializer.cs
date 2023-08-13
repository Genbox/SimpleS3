using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Serializers;

public class DefaultProfileSerializer : IProfileSerializer
{
    public byte[] Serialize<T>(T profile) where T : IProfile
    {
        using MemoryStream ms = new MemoryStream();
        using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
        {
            sw.WriteLine(nameof(profile.Name) + '=' + profile.Name);
            sw.WriteLine(nameof(profile.CreatedOn) + '=' + profile.CreatedOn.UtcDateTime.ToString("O"));
            sw.WriteLine(nameof(profile.Location) + '=' + profile.Location);
            sw.WriteLine(nameof(profile.KeyId) + '=' + profile.KeyId);
            sw.WriteLine(nameof(profile.AccessKey) + '=' + Convert.ToBase64String(profile.AccessKey));
            sw.WriteLine(nameof(profile.RegionCode) + '=' + profile.RegionCode);
        }
        return ms.ToArray();
    }

    public T Deserialize<T>(byte[] data) where T : IProfile
    {
        using MemoryStream ms = new MemoryStream(data, false);
        using StreamReader sr = new StreamReader(ms, Encoding.UTF8);

        T p = Activator.CreateInstance<T>();

        while (!sr.EndOfStream)
        {
            string? str = sr.ReadLine();

            if (str == null)
                break;

            ReadOnlySpan<char> span = str.AsSpan();

            int idx = span.IndexOf('=');
            ReadOnlySpan<char> key = span.Slice(0, idx);
            ReadOnlySpan<char> val = span.Slice(idx + 1);

            switch (key)
            {
                case nameof(IProfile.Name):
                    p.Name = val.ToString();
                    break;
                case nameof(IProfile.CreatedOn):
                    p.CreatedOn = DateTimeOffset.ParseExact(val.ToString(), "O", DateTimeFormatInfo.InvariantInfo);
                    break;
                case nameof(IProfile.Location):
                    p.Location = val.ToString();
                    break;
                case nameof(IProfile.KeyId):
                    p.KeyId = val.ToString();
                    break;
                case nameof(IProfile.AccessKey):
                    p.AccessKey = Convert.FromBase64String(val.ToString());
                    break;
                case nameof(IProfile.RegionCode):
                    p.RegionCode = val.ToString();
                    break;
                default:
                    throw new InvalidOperationException("Invalid key: " + key.ToString());
            }
        }

        return p;
    }
}