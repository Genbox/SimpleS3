using System.Collections.Generic;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public class ListBucketsResponse : BaseResponse
{
    public ListBucketsResponse()
    {
        Buckets = new List<S3Bucket>();
    }

    /// <summary>The owner of the buckets inside <see cref="Buckets" />.</summary>
    public S3Identity Owner { get; internal set; }

    /// <summary>The list of buckets</summary>
    public IList<S3Bucket> Buckets { get; }
}