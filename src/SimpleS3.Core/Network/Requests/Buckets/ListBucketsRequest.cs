﻿using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>This implementation of the GET operation returns a list of all buckets owned by the authenticated sender of
/// the request.</summary>
public class ListBucketsRequest : BaseRequest
{
    public ListBucketsRequest() : base(HttpMethodType.GET) {}
}