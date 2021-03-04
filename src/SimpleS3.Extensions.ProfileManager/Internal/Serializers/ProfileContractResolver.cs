using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Internal.Serializers
{
    /// <summary>This resolver enables the serializer to deserialize properties with internal/private setters</summary>
    internal class ProfileContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);

            if (prop.Writable)
                return prop;

            PropertyInfo? property = member as PropertyInfo;

            if (property == null)
                return prop;

            bool hasPrivateSetter = property.GetSetMethod(true) != null;
            prop.Writable = hasPrivateSetter;

            return prop;
        }
    }
}