using System.IO;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public class GetObjectResponse : HeadObjectResponse, IHasContent, IHasRequestCharged
{
    public Stream Content { get; internal set; }
    public bool RequestCharged { get; internal set; }
}