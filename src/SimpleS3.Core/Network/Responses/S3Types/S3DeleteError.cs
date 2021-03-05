using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3DeleteError : IHasVersionId, IHasObjectKey
    {
        public S3DeleteError(string objectKey, ErrorCode code, string message, string? versionId)
        {
            ObjectKey = objectKey;
            Code = code;
            Message = message;
            VersionId = versionId;
        }

        public string ObjectKey { get; }
        public ErrorCode Code { get; }
        public string Message { get; }
        public string? VersionId { get; }

        public override string ToString()
        {
            return $"Error on {ObjectKey}: {Message}";
        }
    }
}