using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public class DeleteObjectsResponse : BaseResponse, IHasRequestCharged
{
    public IList<S3DeletedObject> Deleted { get; } = new List<S3DeletedObject>();
    public IList<S3DeleteError> Errors { get; } = new List<S3DeleteError>();
    public bool RequestCharged { get; internal set; }
}