using Genbox.SimpleS3.Core.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core
{
    public class BackBlazeB2Config : Config
    {
        public BackBlazeB2Config(IAccessKey credentials, B2Region region)
        {
            Credentials = credentials;
            Region = region;
        }

        public B2Region Region { get; set; }
    }
}