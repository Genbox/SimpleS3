﻿using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public class GetObjectLegalHoldResponse : BaseResponse, IHasRequestCharged
{
    public bool LegalHold { get; internal set; }
    public bool RequestCharged { get; internal set; }
}