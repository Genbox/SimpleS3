using System;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Network.Responses.Properties;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class GetObjectResponse : HeadObjectResponse, IHasContent, IHasRequestCharged, IHasExpiration
    {
        public ContentReader Content { get; internal set; }
        public bool RequestCharged { get; internal set; }
        public DateTimeOffset? LifeCycleExpiresOn { get; internal set; }
        public string LifeCycleRuleId { get; internal set; }
    }
}