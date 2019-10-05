using System.Collections.Generic;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.Errors
{
    [PublicAPI]
    public class XAmzContentSha256MismatchError : GenericError
    {
        internal XAmzContentSha256MismatchError(IDictionary<string, string> lookup) : base(lookup)
        {
            ClientSha256 = lookup["ClientComputedContentSHA256"];
            S3Sha256 = lookup["S3ComputedContentSHA256"];
            RequestId = lookup["RequestId"];
            HostId = lookup["HostId"];
        }

        public string ClientSha256 { get; }
        public string S3Sha256 { get; }
        public string RequestId { get; }
        public string HostId { get; }

        public override string GetExtraData()
        {
            return $"Client SHA256: {ClientSha256} S3Sha256: {S3Sha256}";
        }
    }
}