﻿using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public class DeleteObjectsResponse : BaseResponse, IHasRequestCharged
{
    public DeleteObjectsResponse()
    {
        Deleted = new List<S3DeletedObject>();
        Errors = new List<S3DeleteError>();
    }

    public IList<S3DeletedObject> Deleted { get; }
    public IList<S3DeleteError> Errors { get; }
    public bool RequestCharged { get; internal set; }
}