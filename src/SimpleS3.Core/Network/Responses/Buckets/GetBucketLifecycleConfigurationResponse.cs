using System.Collections.Generic;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public class GetBucketLifecycleConfigurationResponse : BaseResponse
{
    public GetBucketLifecycleConfigurationResponse()
    {
        Rules = new List<S3Rule>();
    }

    public IList<S3Rule> Rules { get; }
}