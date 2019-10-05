using System.Collections.Generic;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.Errors
{
    [PublicAPI]
    public class MethodNotAllowedError : GenericError
    {
        internal MethodNotAllowedError(IDictionary<string, string> lookup) : base(lookup)
        {
            Method = lookup["Method"];
            ResourceType = lookup["ResourceType"];
            RequestId = lookup["RequestId"];
            HostId = lookup["HostId"];
        }

        public string Method { get; }
        public string ResourceType { get; }
        public string RequestId { get; }
        public string HostId { get; }

        public override string GetExtraData()
        {
            return $"Method: {Method} ResourceType: {ResourceType}";
        }
    }
}