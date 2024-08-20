using Genbox.SimpleS3.Core.Builders;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

public interface IHasTags
{
    /// <summary>
    /// Specifies a set of one or more tags to associate with the object. These tags are stored in the tagging subresource that is associated with the object. To specify tags on
    /// an object, the requester must have s3:PutObjectTagging included in the list of permitted actions in their IAM policy.
    /// </summary>
    TagBuilder Tags { get; }
}