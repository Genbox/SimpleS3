using Genbox.SimpleS3.Core.Network.Responses.Properties;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class RestoreObjectResponse : BaseResponse, IHasRequestCharged
    {
        public string RestoreOutputPath { get; internal set; }
        public bool RequestCharged { get; internal set; }
    }
}