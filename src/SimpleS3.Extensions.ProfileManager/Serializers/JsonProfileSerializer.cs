using System.IO;
using System.Text;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Newtonsoft.Json;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Serializers
{
    public class JsonProfileSerializer : IProfileSerializer
    {
        private readonly JsonSerializer _serializer;

        public JsonProfileSerializer()
        {
            JsonSerializerSettings s = new JsonSerializerSettings();
            s.Formatting = Formatting.Indented;
            s.NullValueHandling = NullValueHandling.Ignore; //We don't need to keep null values
            s.DateTimeZoneHandling = DateTimeZoneHandling.Utc; //Handle the dates as UTC
            s.ContractResolver = new ProfileContractResolver();
            _serializer = JsonSerializer.Create(s);
        }

        public byte[] Serialize<T>(T profile) where T : IProfile
        {
            // Serialize as a single object
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8, 4096, true))
                using (JsonTextWriter jw = new JsonTextWriter(sw))
                {
                    jw.CloseOutput = false;
                    _serializer.Serialize(jw, profile);
                }

                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] data) where T : IProfile
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (StreamReader sr = new StreamReader(ms))
            using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
                return _serializer.Deserialize<T>(jsonTextReader);
        }
    }
}