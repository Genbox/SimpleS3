using Genbox.SimpleS3.Core.Network.Responses.Properties;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class RestoreObjectResponse : BaseResponse, IHasRequestCharged
    {
        public bool RequestCharged { get; internal set; }
        public string RestoreOutputPath { get; internal set; }
    }
}