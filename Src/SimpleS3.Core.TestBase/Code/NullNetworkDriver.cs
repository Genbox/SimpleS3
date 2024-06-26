﻿using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.TestBase.Code;

public class NullNetworkDriver : INetworkDriver
{
    public string? LastUrl { get; private set; }

    public Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken cancellationToken = default) where T : IResponse
    {
        LastUrl = url;
        return Task.FromResult(new HttpResponse());
    }
}