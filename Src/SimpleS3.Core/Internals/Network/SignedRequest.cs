using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests;

namespace Genbox.SimpleS3.Core.Internals.Network;

internal sealed class SignedRequest : BaseRequest
{
    public SignedRequest(HttpMethodType method) : base(method) {}
}