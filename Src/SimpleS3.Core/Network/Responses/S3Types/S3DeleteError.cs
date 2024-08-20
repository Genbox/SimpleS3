using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3DeleteError(string objectKey, ErrorCode code, string message, string? versionId) : IHasVersionId, IHasObjectKey
{
    public ErrorCode Code { get; } = code;
    public string Message { get; } = message;
    public string ObjectKey { get; } = objectKey;
    public string? VersionId { get; } = versionId;
    public override string ToString() => $"Error on {ObjectKey}: {Message}";
}