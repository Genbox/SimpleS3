using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    /// <summary>The Filter is used to identify objects that a Lifecycle Rule applies to. A Filter must have exactly one of Prefix, Tag, or And specified.</summary>
    public class S3Filter : S3AndCondition
    {
        public S3Filter()
        {
            Conditions = new List<S3AndCondition>();
        }

        /// <summary>
        /// This is used in a Lifecycle Rule Filter to apply a logical AND to two or more predicates. The Lifecycle Rule will apply to any object
        /// matching all of the predicates configured inside the And operator.
        /// </summary>
        public IList<S3AndCondition> Conditions { get; }
    }
}