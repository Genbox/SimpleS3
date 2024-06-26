﻿using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests;

public abstract class SignedBaseRequest : BaseRequest
{
    protected SignedBaseRequest(HttpMethodType method) : base(method) {}

    public string Url { get; internal set; } = null!;
}