using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3DeleteError</c> API.</summary>
public class S3DeleteError(string objectKey, ErrorCode code, string message, string? versionId) : IHasVersionId, IHasObjectKey
{
    /// <summary>Provides the <c>Code</c> API.</summary>
    public ErrorCode Code { get; } = code;
    /// <summary>Provides the <c>Message</c> API.</summary>
    public string Message { get; } = message;
    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; } = objectKey;
    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; } = versionId;
    /// <summary>Provides the <c>ToString</c> API.</summary>
    public override string ToString() => $"Error on {ObjectKey}: {Message}";
}