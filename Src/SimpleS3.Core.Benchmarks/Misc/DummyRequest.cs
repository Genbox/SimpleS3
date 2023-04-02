using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests;

namespace Genbox.SimpleS3.Core.Benchmarks.Misc;

public class DummyRequest : BaseRequest
{
    public DummyRequest() : base(HttpMethodType.GET) {}
}