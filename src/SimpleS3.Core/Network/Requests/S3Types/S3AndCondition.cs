using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    public class S3AndCondition
    {
        /// <summary>Prefix identifying one or more objects to which the rule applies. Set to empty string to apply to the whole bucket.</summary>
        public string Prefix { get; set; }

        /// <summary>This tag must exist in the object's tag set in order for the rule to apply.</summary>
        public KeyValuePair<string, string>? Tag { get; set; }
    }
}