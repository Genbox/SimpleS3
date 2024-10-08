﻿using Genbox.SimpleS3.Core.Common;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.HttpClient;

[PublicAPI]
public class HttpClientConfig
{
    /// <summary>Set to 'true' if you want the proxy that is defined to be used. Otherwise set it to 'false'. Defaults to 'false'.</summary>
    public bool UseProxy { get; set; }

    /// <summary>Use this to set a proxy to which all requests should be sent through.</summary>
    public string? Proxy { get; set; }

    public HttpVersion HttpVersion { get; set; }
}